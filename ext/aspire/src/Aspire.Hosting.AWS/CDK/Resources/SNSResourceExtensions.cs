using Amazon.CDK.AWS.SNS;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.AWS;
using Aspire.Hosting.AWS.CDK;

namespace Aspire.Hosting;

/// <summary>
/// Provides extension methods for adding Amazon SNS resources to the application model.
/// </summary>
public static class SNSResourceExtensions
{
    /// <summary>
    /// Adds an Amazon SNS topic.
    /// </summary>
    /// <param name="builder">The builder for the distributed application.</param>
    /// <param name="name">The name of the resource.</param>
    /// <param name="props">The properties of the topic.</param>
    public static IResourceBuilder<IConstructResource<Topic>> AddSNSTopic(this IResourceBuilder<IResourceWithConstruct> builder, string name, ITopicProps? props = null)
    {
        return builder.AddConstruct(name, scope => new Topic(scope, name, props));
    }

    /// <summary>
    /// Adds a reference of an Amazon SNS topic to a project. The output parameters of the Amazon DynamoDB table are added to the project IConfiguration.
    /// </summary>
    /// <param name="builder">The builder for the resource.</param>
    /// <param name="topic">The Amazon SNS topic resource.</param>
    /// <param name="configSection">The optional config section in IConfiguration to add the output parameters.</param>
    public static IResourceBuilder<TDestination> WithReference<TDestination>(this IResourceBuilder<TDestination> builder, IResourceBuilder<IConstructResource<Topic>> topic, string configSection = Constants.DefaultConfigSection)
        where TDestination : IResourceWithEnvironment
    {
        var prefix = configSection.Replace(':', '_');
        return builder.WithEnvironment($"{prefix}__TopicArn", topic, t => t.TopicArn);
    }
}