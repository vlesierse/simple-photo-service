using Amazon.CDK;
using Amazon.CDK.AWS.Cognito;
using Amazon.CDK.AWS.DynamoDB;
using Amazon.CDK.AWS.S3;
using Attribute = Amazon.CDK.AWS.DynamoDB.Attribute;

var builder = DistributedApplication.CreateBuilder(args).WithAWSCDK();
var config = builder.AddAWSSDKConfig().WithProfile("default");

var stack = builder.AddStack("stack", stackName: "SimplePhotoService").WithReference(config);
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
            PartitionKey = new Attribute { Name = "owner", Type = AttributeType.STRING },
            SortKey = new Attribute { Name = "ownerSK", Type = AttributeType.STRING },
            ProjectionType = ProjectionType.ALL
        });
var topic = stack.AddSNSTopic("topic");
var bucket = stack.AddS3Bucket("bucket", new BucketProps { RemovalPolicy = RemovalPolicy.DESTROY });
var userPool = stack.AddCognitoUserPool("userpool", new UserPoolProps
{
    UserPoolName = $"SimplePhotoService",
    SignInAliases = new SignInAliases { Email = true, Username = false },
    SelfSignUpEnabled = true,
    RemovalPolicy = RemovalPolicy.DESTROY
});
var userPoolClient = userPool.AddClient("client", new UserPoolClientOptions());

var api = builder.AddProject<Projects.SimplePhotoService_Api>("api")
    .WithReference(table, "AWS::Resources::Table")
    .WithReference(topic, "AWS::Resources::Topic")
    .WithReference(bucket, "AWS::Resources::Bucket");

builder.AddNpmApp("frontend", "../../frontend", "dev")
    .WithEnvironment("VITE_API_HTTP", api.GetEndpoint("http"))
    .WithEnvironment("VITE_AWS_USER_POOL_ID", userPool, c => c.UserPoolId)
    .WithEnvironment("VITE_AWS_USER_POOL_CLIENT_ID", userPoolClient, c => c.UserPoolClientId)
    .WithHttpEndpoint(env: "PORT", port: 5173);

builder.Build().Run();