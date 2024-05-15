// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Amazon.CloudFormation.Model;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.AWS;
using Aspire.Hosting.AWS.CDK;
using Aspire.Hosting.AWS.CloudFormation;
using Constructs;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using Stack = Amazon.CDK.Stack;
using StackResource = Aspire.Hosting.AWS.CDK.StackResource;

namespace Aspire.Hosting;

/// <summary>
///
/// </summary>
public static class CDKExtensions
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static ICDKApplicationBuilder WithAWSCDK(this IDistributedApplicationBuilder builder)
    {
        return new CDKApplicationBuilder(builder);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="name"></param>
    /// <param name="stackName"></param>
    /// <returns></returns>
    public static IResourceBuilder<IStackResource> AddStack(this ICDKApplicationBuilder builder, string name, string? stackName = null)
    {
        var stack = new Stack(builder.App, stackName ?? name);
        return builder.AddResource(new StackResource(name, stack));
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="stackBuilder"></param>
    /// <returns></returns>
    public static IResourceBuilder<IStackResource<T>> AddStack<T>(this ICDKApplicationBuilder builder, StackBuilderDelegate<T> stackBuilder)
        where T : Stack
    {
        var stack = stackBuilder(builder.App);
        return builder.AddResource(new StackResource<T>(stack.StackName, stack));
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="name"></param>
    /// <param name="constructBuilder"></param>
    /// <returns></returns>
    public static IResourceBuilder<IConstructResource<T>> AddConstruct<T>(this IResourceBuilder<IResourceWithConstruct> builder, string name, ConstructBuilderDelegate<T> constructBuilder)
        where T : Construct
    {
        var construct = constructBuilder((Construct)builder.Resource.Construct);
        return builder.AddResource(parent => new ConstructResource<T>(name, construct, builder.Resource));
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="name"></param>
    /// <param name="output"></param>
    /// <typeparam name="TStack"></typeparam>
    /// <returns></returns>
    public static IResourceBuilder<IStackResource<TStack>> WithOutput<TStack>(
        this IResourceBuilder<IStackResource<TStack>> builder,
        string name, ConstructOutputDelegate<TStack> output)
        where TStack : Stack
    {
        return builder.WithAnnotation(new ConstructOutputAnnotation<TStack>(name, output));
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="name"></param>
    /// <param name="output"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IResourceBuilder<IConstructResource<T>> WithOutput<T>(
        this IResourceBuilder<IConstructResource<T>> builder,
        string name, ConstructOutputDelegate<T> output)
        where T : Construct
    {
        return builder.WithAnnotation(new ConstructOutputAnnotation<T>(name, output));
    }

    /// <summary>
    /// Gets a reference to an output from the CloudFormation stack.
    /// </summary>
    /// <param name="builder">The resource builder.</param>
    /// <param name="name">The name of the output.</param>
    /// <param name="outputDelegate"></param>
    public static StackOutputReference GetOutput<T>(this IResourceBuilder<IConstructResource<T>> builder, string name, ConstructOutputDelegate<T> outputDelegate)
        where T : Construct
    {
        builder.WithAnnotation(new ConstructOutputAnnotation<T>(name, outputDelegate));
        return new StackOutputReference(builder.Resource.Construct.StackUniqueId() + name, builder.Resource.FindParentOfType<StackResource>());
    }

    /// <summary>
    /// The AWS SDK service client configuration used to create the CloudFormation service client.
    /// </summary>
    /// <param name="builder">The resource builder.</param>
    /// <param name="awsSdkConfig">The name of the AWS credential profile.</param>
    public static IResourceBuilder<T> WithReference<T>(this IResourceBuilder<T> builder, IAWSSDKConfig awsSdkConfig)
        where T : IStackResource
    {
        builder.Resource.AWSSDKConfig = awsSdkConfig;
        return builder;
    }

    /// <summary>
    /// Add a reference of a CloudFormations stack to a project. The output parameters of the CloudFormation stack are added to the project IConfiguration.
    /// </summary>
    /// <typeparam name="TDestination"></typeparam>
    /// <param name="builder">The resource builder.</param>
    /// <param name="stack">The stack resource.</param>
    /// <param name="configSection">The config section in IConfiguration to add the output parameters.</param>
    /// <returns></returns>
    public static IResourceBuilder<TDestination> WithReference<TDestination>(this IResourceBuilder<TDestination> builder, IResourceBuilder<IStackResource> stack, string configSection = Constants.DefaultConfigSection)
        where TDestination : IResourceWithEnvironment
    {
        stack.WithAnnotation(new CloudFormationReferenceAnnotation(builder.Resource.Name));

        builder.WithEnvironment(async ctx =>
        {
            if (ctx.ExecutionContext.IsPublishMode)
            {
                return;
            }

            if (stack.Resource.AWSSDKConfig != null)
            {
                SdkUtilities.ApplySDKConfig(ctx, stack.Resource.AWSSDKConfig, false);
            }

            if (stack.Resource.ProvisioningTaskCompletionSource is not null)
            {
                ctx.Logger?.LogInformation("Waiting on Stack resource {Name} ...", stack.Resource.Name);
                await stack.Resource.ProvisioningTaskCompletionSource.Task.WaitAsync(ctx.CancellationToken).ConfigureAwait(false);
            }

            var stackOutputs = stack.Resource.Outputs;
            if (stackOutputs == null)
            {
                return;
            }

            configSection = configSection.Replace(':', '_');

            var processedOutputs = new List<Output>();
            foreach (var construct in stack.Resource.ListChildren(builder.ApplicationBuilder.Resources.OfType<IConstructResource>()))
            {
                var constructId = construct.Construct.StackUniqueId();
                var outputs = stackOutputs.Where(o => o.OutputKey.StartsWith(constructId)).ToList();

                var prefix = configSection.Replace(':', '_');
                var parents = construct.ListParents(builder.ApplicationBuilder.Resources.OfType<IConstructResource>()).ToArray();
                if (parents.Length != 0)
                {
                    prefix += "__" + string.Join("__", parents.Select(r => r.Name));
                }
                prefix += "__" + construct.Name;
                foreach (var output in outputs)
                {
                    var envName = $"{prefix}__{output.OutputKey.TrimStart(constructId)}";
                    ctx.EnvironmentVariables[envName] = output.OutputValue;

                }
                processedOutputs.AddRange(outputs);
            }
            foreach (var output in stackOutputs.Except(processedOutputs))
            {
                var envName = $"{configSection}__{output.OutputKey}";
                ctx.EnvironmentVariables[envName] = output.OutputValue;
            }
        });

        return builder;
    }

    /// <summary>
    /// Add a reference of a construct to a project. The output parameters of the CloudFormation stack are added as environment variable.
    /// </summary>
    /// <param name="builder">The resource builder.</param>
    /// <param name="name">The name of the environment variable.</param>
    /// <param name="source">The construct resource.</param>
    /// <param name="outputDelegate">The construct output resolver.</param>
    /// <param name="outputName">The name of the construct output.</param>
    public static IResourceBuilder<TDestination> WithEnvironment<TDestination, TConstruct>(this IResourceBuilder<TDestination> builder, string name, IResourceBuilder<IConstructResource<TConstruct>> source, ConstructOutputDelegate<TConstruct> outputDelegate, string? outputName = default)
        where TConstruct : IConstruct
        where TDestination : IResourceWithEnvironment
    {
        outputName ??= name.Replace("_", string.Empty);
        if (!source.Resource.Annotations.OfType<IConstructOutputAnnotation>().Where(annotation => annotation.OutputName == outputName).Any())
        {
            source.WithAnnotation(new ConstructOutputAnnotation<TConstruct>(outputName, outputDelegate));
        }
        return builder.WithEnvironment(name, new StackOutputReference(source.Resource.Construct.StackUniqueId() + outputName, source.Resource.FindParentOfType<StackResource>()));
    }
}
