using Amazon;
using Amazon.BedrockRuntime;
using Amazon.DynamoDBv2;
using Amazon.Rekognition;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.SQS.Model;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using SimplePhotoService.Controller.Handlers;
using SimplePhotoService.Controller.Processing;
using SimplePhotoService.Infrastructure.Domain;
using SimplePhotoService.Infrastructure.Storage;
using XaasKit.Amazon.DynamoDB.Repositories;

namespace SimplePhotoService.Controller.Tests;

public class UnitTest1
{
    [Fact]
    public Task Test1()
    {
        const string message =
            "{\"Records\":[{\"eventVersion\":\"2.1\",\"eventSource\":\"aws:s3\",\"awsRegion\":\"eu-west-1\",\"eventTime\":\"2024-10-12T08:13:17.401Z\",\"eventName\":\"ObjectCreated:Put\",\"userIdentity\":{\"principalId\":\"AWS:AROAQR5XBCJKZLA56NFRF:vinles-Isengard\"},\"requestParameters\":{\"sourceIPAddress\":\"86.83.239.52\"},\"responseElements\":{\"x-amz-request-id\":\"3R8TBBSJWJ6EH2EZ\",\"x-amz-id-2\":\"RcrZMc/DMjuqOWL6k/Lfik/Mun+tFoBX/WWFmhWU60NXZ+lPHK/VgsSb/fcVVd103PrWdWJV1gy2dcbk7qfV/LRwUH+JxC2E\"},\"s3\":{\"s3SchemaVersion\":\"1.0\",\"configurationId\":\"ZDk1NDk5NTgtNDAwZC00OTUwLThjYWItNWJlOTMwZDgzOTcw\",\"bucket\":{\"name\":\"simplephotoservice-bucket43879c71-kcygnntjqgnn\",\"ownerIdentity\":{\"principalId\":\"A1YVC42LT99G34\"},\"arn\":\"arn:aws:s3:::simplephotoservice-bucket43879c71-kcygnntjqgnn\"},\"object\":{\"key\":\"upload/8bb0c8ea-1be7-40bd-9156-fe4b64388af0/794ad780-41b9-40d1-abfc-0c27f4f09350\",\"size\":269828,\"eTag\":\"8f8b1afe52384eb848e031487737fd87\",\"sequencer\":\"00670A2F9D59B86DCA\"}}}]}";
        var store = new S3ObjectStore(new AmazonS3Client(CreateServiceCredentials<AmazonS3Config>("vinles+labs-Admin")), Options.Create(new S3ObjectStoreOptions { BucketName = "simplephotoservice-bucket43879c71-kcygnntjqgnn"}));
        var repository = new PhotoRepository(new AmazonDynamoDBClient(CreateServiceCredentials<AmazonDynamoDBConfig>("vinles+labs-Admin")), Options.Create(new DynamoDBOptions { TableName = "SimplePhotoService-table8235A42E-BGT3YMP7P8ZF"}));
        var rekognitionClient = new AmazonRekognitionClient(CreateServiceCredentials<AmazonRekognitionConfig>("vinles+labs-Admin"));
        var bedrockClient = new AmazonBedrockRuntimeClient(CreateServiceCredentials<AmazonBedrockRuntimeConfig>("vinles+labs-Admin"), RegionEndpoint.USWest2);
        // Processors
        var processors = new List<IImageProcessor>
        {
            new ModerationProcessor(rekognitionClient, NullLogger<ModerationProcessor>.Instance),
            new SmartCropProcessor(Options.Create(new SmartCropOptions { CropLabel = "Food" })),
            new CleanProcessor(bedrockClient, Options.Create(new CleanOptions())),
            new LabelDetectionProcessor(rekognitionClient, Options.Create(new LabelDetectionOptions{ ConfidenceThreshold = 80}), NullLogger<LabelDetectionProcessor>.Instance),

        };

        var handler = new S3EventMessageHandler(store, repository, processors, new NullLogger<S3EventMessageHandler>());
        
        return handler.Handle(new Message{ Body = message});
    }
    
    internal AWSCredentials CreateServiceCredentials<T>(string? profile = null) where T : ClientConfig, new()
    {
        var config = new T();

        if (!string.IsNullOrEmpty(profile))
        {
            config.Profile = new Profile(profile);
        }

        return FallbackCredentialsFactory.GetCredentials(config);
    }
}
