// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Amazon.CDK;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.AWS;
using Aspire.Hosting.AWS.CDK;
using Aspire.Hosting.AWS.CloudFormation;
using Aspire.Hosting.Lifecycle;
using Constructs;
using Stack = Amazon.CDK.Stack;
using StackResource = Aspire.Hosting.AWS.CDK.StackResource;

namespace Aspire.Hosting;

/// <summary>
/// Extension methods for adding AWS CDK as a provisioning resources.
/// </summary>
public static class CDKExtensions
{
    internal static IDistributedApplicationBuilder AddAWSCDKPublishing(this IDistributedApplicationBuilder builder)
    {
        builder.Services.TryAddLifecycleHook<CDKLifecycleHook>();
        return builder;
    }

    /// <summary>
    /// Add an AWS CDK app with stack for provisioning application resources.
    /// </summary>
    /// <param name="builder">The <see cref="IDistributedApplicationBuilder"/>.</param>
    /// <param name="name"></param>
    /// <param name="stackName"></param>
    /// <param name="props"></param>
    /// <returns></returns>
    public static IResourceBuilder<ICDKResource> AddAWSCDK(this IDistributedApplicationBuilder builder, string name, string? stackName = null, IAppProps? props = null)
    {

        builder
            .AddAWSCDKPublishing()
            .AddAWSProvisioning();
        var resource = new CDKResource(name, stackName, props);
        return builder
            .AddResource(resource)
            .WithInitialState(new()
            {
                Properties = [],
                ResourceType = GetResourceType<Stack>(resource),
            })
            .WithManifestPublishingCallback(resource.WriteToManifest);
    }

    /// <summary>
    /// Adds an AWS CDK stack as resource.
    /// </summary>
    /// <param name="builder">The AWS CDK Resource builder.</param>
    /// <param name="name">The name of the stack resource.</param>
    /// <returns></returns>
    public static IResourceBuilder<IStackResource> AddStack(this IResourceBuilder<ICDKResource> builder, string name)
        => AddStack(builder, name, $"{builder.Resource.StackName}-{name}");

    /// <summary>
    /// Adds an AWS CDK stack as resource.
    /// </summary>
    /// <param name="builder">The AWS CDK Resource builder.</param>
    /// <param name="name">The name of the stack resource.</param>
    /// <param name="stackName">Cloud Formation stack same if different from the resource name.</param>
    /// <returns></returns>
    public static IResourceBuilder<IStackResource> AddStack(this IResourceBuilder<ICDKResource> builder, string name,
        string stackName)
    {
        var parent = builder.Resource;
        var resource = new StackResource(name, new Stack(parent.App, stackName), parent);
        return builder.ApplicationBuilder
            .AddResource(resource)
            .WithInitialState(new()
            {
                Properties = [],
                ResourceType = GetResourceType<Stack>(resource),
            })
            .WithManifestPublishingCallback(resource.WriteToManifest);
    }

    /// <summary>
    /// Adds and build an AWS CDK stack as resource.
    /// </summary>
    /// <param name="builder">The AWS CDK Resource builder.</param>
    /// <param name="name">The name of the resource.</param>
    /// <param name="stackBuilder">The stack builder delegate.</param>
    /// <returns></returns>
    public static IResourceBuilder<IStackResource<T>> AddStack<T>(this IResourceBuilder<ICDKResource> builder,
        string name, ConstructBuilderDelegate<T> stackBuilder)
        where T : Stack
    {
        var parent = builder.Resource;
        var resource = new StackResource<T>(name, stackBuilder(parent.App), parent);
        return builder.ApplicationBuilder
            .AddResource(resource)
            .WithInitialState(new()
            {
                Properties = [],
                ResourceType = GetResourceType<Stack>(resource),
            })
            .WithManifestPublishingCallback(resource.WriteToManifest);
    }

    /// <summary>
    /// Adds and build an AWS CDK construct as resource.
    /// </summary>
    /// <param name="builder">The construct resource builder.</param>
    /// <param name="name">The name of the resource.</param>
    /// <param name="constructBuilder">The construct builder delegate.</param>
    /// <returns></returns>
    public static IResourceBuilder<IConstructResource<T>> AddConstruct<T>(
        this IResourceBuilder<IResourceWithConstruct> builder, string name,
        ConstructBuilderDelegate<T> constructBuilder)
        where T : Construct
    {
        var parent = builder.Resource;
        var resource = new ConstructResource<T>(name, constructBuilder((Construct)parent.Construct), parent);
        return builder.ApplicationBuilder
            .AddResource(resource)
            .WithInitialState(new()
            {
                Properties = [],
                ResourceType = GetResourceType<Construct>(resource),
            })
            .WithManifestPublishingCallback(resource.WriteToManifest);
    }

    /// <summary>
    /// Adds a stack reference to an output from the CloudFormation stack.
    /// </summary>
    /// <param name="builder">The stack resource builder.</param>
    /// <param name="name">The name of the output.</param>
    /// <param name="output">The construct output delegate.</param>
    /// <typeparam name="TStack"></typeparam>
    /// <example>
    /// The following example shows creating a custom stack and reference the exposed ServiceUrl property
    /// in a project.
    /// <code>
    /// var service = app
    ///     .AddStack("service", scope => new ServiceStack(scope, "ServiceStack")
    ///     .AddOutput("ServiceUrl", stack => stack.Service.ServiceUrl);
    /// var api = builder.AddProject&lt;Projects.Api&gt;("api")
    ///     .AddReference(service);
    /// </code>
    /// </example>
    public static IResourceBuilder<IStackResource<TStack>> AddOutput<TStack>(
        this IResourceBuilder<IStackResource<TStack>> builder,
        string name, ConstructOutputDelegate<TStack> output)
        where TStack : Stack
    {
        return builder.WithAnnotation(new ConstructOutputAnnotation<TStack>(name, output));
    }

    /// <summary>
    /// Adds a construct reference to an output from the CloudFormation stack.
    /// </summary>
    /// <param name="builder">The construct resource builder.</param>
    /// <param name="name">The name of the output.</param>
    /// <param name="output">The construct output delegate.</param>
    /// <typeparam name="T"></typeparam>
    /// <example>
    /// The following example shows creating a custom construct and reference the exposed ServiceUrl property
    /// in a project.
    /// <code lang="C#">
    /// var service = stack
    ///     .AddConstruct("service", scope => new Service(scope, "service")
    ///     .AddOutput("ServiceUrl", construct => construct.ServiceUrl);
    /// var api = builder.AddProject&lt;Projects.Api&gt;("api")
    ///     .AddReference(service);
    /// </code>
    /// </example>
    public static IResourceBuilder<IConstructResource<T>> AddOutput<T>(
        this IResourceBuilder<IConstructResource<T>> builder,
        string name, ConstructOutputDelegate<T> output)
        where T : Construct
    {
        return builder.WithAnnotation(new ConstructOutputAnnotation<T>(name, output));
    }

    /// <summary>
    /// Gets a reference to an output from the CloudFormation stack.
    /// </summary>
    /// <param name="builder">The construct resource builder.</param>
    /// <param name="name">The name of the output.</param>
    /// <param name="output">The construct output delegate.</param>
    public static StackOutputReference GetOutput<T>(this IResourceBuilder<IConstructResource<T>> builder, string name, ConstructOutputDelegate<T> output)
        where T : Construct
    {
        builder.WithAnnotation(new ConstructOutputAnnotation<T>(name, output));
        return new StackOutputReference(builder.Resource.Construct.StackUniqueId() + name, builder.Resource.FindParentOfType<StackResource>());
    }

    /// <summary>
    /// Adds a reference of an AWS CDK construct to a project.The output parameters of the construct are added to the project IConfiguration.
    /// </summary>
    /// <param name="builder">The builder for the resource.</param>
    /// <param name="construct">The construct resource.</param>
    /// <param name="outputDelegate">The construct output delegate.</param>
    /// <param name="outputName">The name of the construct output</param>
    /// <param name="configSection">The optional config section in IConfiguration to add the output parameters.</param>
    public static IResourceBuilder<TDestination> WithReference<TDestination, TConstruct>(this IResourceBuilder<TDestination> builder, IResourceBuilder<IConstructResource<TConstruct>> construct, ConstructOutputDelegate<TConstruct> outputDelegate, string outputName, string? configSection = null)
        where TConstruct : IConstruct
        where TDestination : IResourceWithEnvironment
    {
        configSection ??= $"{Constants.DefaultConfigSection}:{construct.Resource.Name}";
        var prefix = configSection.ToEnvironmentVariables();
        return builder.WithEnvironment($"{prefix}__{outputName}", construct, outputDelegate, outputName);
    }

    /// <summary>
    /// Add an environment variable with a reference of a AWS CDK construct to a project. The output parameters of the CloudFormation stack are added to the project IConfiguration.
    /// </summary>
    /// <param name="builder">The resource builder.</param>
    /// <param name="name">The name of the environment variable.</param>
    /// <param name="construct">The construct resource.</param>
    /// <param name="outputDelegate">The construct output delegate.</param>
    /// <param name="outputName">The name of the construct output</param>
    /// /// <example>
    /// The following example shows creating a custom construct and reference the exposed ServiceUrl property
    /// in a project as environment variable.
    /// <code lang="C#">
    /// var service = stack.AddConstruct("service", scope => new Service(scope, "service");
    /// var api = builder.AddProject&lt;Projects.Api&gt;("api")
    ///     .WithEnvironment("Service_ServiceUrl", service, s => s.ServiceUrl);
    /// </code>
    /// </example>
    public static IResourceBuilder<TDestination> WithEnvironment<TDestination, TConstruct>(this IResourceBuilder<TDestination> builder, string name, IResourceBuilder<IConstructResource<TConstruct>> construct, ConstructOutputDelegate<TConstruct> outputDelegate, string? outputName = default)
        where TConstruct : IConstruct
        where TDestination : IResourceWithEnvironment
    {
        outputName ??= name.Replace("_", string.Empty);
        if (construct.Resource.Annotations.OfType<IConstructOutputAnnotation>().All(annotation => annotation.OutputName != outputName))
        {
            construct.WithAnnotation(new ConstructOutputAnnotation<TConstruct>(outputName, outputDelegate));
        }
        construct.WithAnnotation(new ConstructReferenceAnnotation(builder.Resource.Name, outputName));
        return builder.WithEnvironment(name, new StackOutputReference(construct.Resource.Construct.StackUniqueId() + outputName, construct.Resource.FindParentOfType<IStackResource>()));
    }

    private static string GetResourceType<T>(IResourceWithConstruct constructResource)
        where T : Construct
    {
        var constructType = constructResource.Construct.GetType();
        var baseConstructType = typeof(T);
        return constructType == baseConstructType ? baseConstructType.Name : constructType.Name;
    }
}
