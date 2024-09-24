using Amazon.CDK;
using Amazon.CDK.AWS.Cognito;
using Amazon.CDK.AWS.DynamoDB;
using Amazon.CDK.AWS.S3;
using Attribute = Amazon.CDK.AWS.DynamoDB.Attribute;

var builder = DistributedApplication.CreateBuilder(args);
var config = builder.AddAWSSDKConfig().WithProfile("default");

var stack = builder.AddAWSCDKStack("stack", stackName: "SimplePhotoService").WithReference(config);
var table = stack
    .AddDynamoDBTable("table", new TableProps
    {
        PartitionKey = new Attribute { Name = "PK", Type = AttributeType.STRING },
        SortKey = new Attribute { Name = "SK", Type = AttributeType.STRING },
        BillingMode = BillingMode.PAY_PER_REQUEST,
        RemovalPolicy = RemovalPolicy.DESTROY
    })
        .AddGlobalSecondaryIndex(new GlobalSecondaryIndexProps {
            IndexName = "OwnerIndex",
            PartitionKey = new Attribute { Name = "OwnerId", Type = AttributeType.STRING },
            SortKey = new Attribute { Name = "OwnerSK", Type = AttributeType.STRING },
            ProjectionType = ProjectionType.ALL
        });
var bucketNotifications = stack.AddSQSQueue("queue");
var bucket = stack.AddS3Bucket("bucket", new BucketProps
    {
        Cors = [
            new CorsRule
            {
                AllowedMethods = [ HttpMethods.PUT, HttpMethods.GET ],
                AllowedHeaders = ["*"],
                AllowedOrigins = ["*"]
            }
        ],
        RemovalPolicy = RemovalPolicy.DESTROY
    })
    .AddObjectCreatedNotification(bucketNotifications, new NotificationKeyFilter { Prefix = "upload/"});

var userPool = stack.AddCognitoUserPool("userpool", new UserPoolProps
{
    UserPoolName = $"SimplePhotoService",
    SignInAliases = new SignInAliases { Email = true, Username = false },
    SelfSignUpEnabled = true,
    RemovalPolicy = RemovalPolicy.DESTROY
});
var userPoolClient = userPool.AddClient("client", new UserPoolClientOptions());

stack.AddS3Bucket("test", new BucketProps() { RemovalPolicy = RemovalPolicy.DESTROY });

var api = builder.AddProject<Projects.SimplePhotoService_Api>("api")
    .WithReference(table, "AWS::Resources::Table")
    .WithReference(bucket, "AWS::Resources::Bucket")
    .WithReference(userPool, "AWS::Resources::Cognito");

builder.AddProject<Projects.SimplePhotoService_Controller>("controller")
    .WithReference(table, "AWS::Resources::Table")
    .WithReference(bucket, "AWS::Resources::Bucket")
    .WithReference(bucketNotifications, "AWS::Resources::Queue");

builder.AddNpmApp("frontend", "../../frontend", "dev")
    .WithEnvironment("VITE_API_HTTP", api.GetEndpoint("http"))
    .WithEnvironment("VITE_AWS_USER_POOL_ID", userPool, c => c.UserPoolId, "UserPoolId")
    .WithEnvironment("VITE_AWS_USER_POOL_CLIENT_ID", userPoolClient, c => c.UserPoolClientId)
    .WithHttpEndpoint(env: "PORT", port: 5173);

builder.Build().Run();