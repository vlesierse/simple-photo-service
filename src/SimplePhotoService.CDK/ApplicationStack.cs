using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.ECS;
using Amazon.CDK.AWS.ECS.Patterns;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.S3;
using Amazon.CDK.AWS.S3.Notifications;
using Amazon.CDK.AWS.SQS;
using Constructs;
using HealthCheck = Amazon.CDK.AWS.ElasticLoadBalancingV2.HealthCheck;
using Stack = Amazon.CDK.Stack;

namespace SimplePhotoService.CDK;

public class ApplicationStackProps : StackProps;

public interface IApplicationStack;

public class ApplicationStack : Stack, IApplicationStack
{
    private readonly Table _table;
    private readonly Bucket _bucket;
    private readonly UserPool _userPool;
    private readonly IEnumerable<IUserPoolClient> _userPoolWebClients;
    
    public string TableName { get; init; }
    
    public ApplicationStack(Construct scope, string id, ApplicationStackProps props)
        : base(scope, id, props)
    {
        var environmentName = this.GetEnvironmentName();
        
        _table = CreateTable();
        _bucket = CreateBucket();
        (_userPool, _userPoolWebClients) = CreateUserPool();
        var cluster = CreateCluster();
        CreateApiService(cluster);
        CreateControllerService(cluster, _bucket);
        // Expose Stack properties for external referencing
        TableName = _table.TableName;
    }

    private ICluster CreateCluster()
    {
        var environmentName = this.GetEnvironmentName();
        var vpc = new Vpc(this, "VPC");
        return new Cluster(this, "Cluster", new ClusterProps
        {
            ClusterName = $"SimplePhotoService-{environmentName}",
            Vpc = vpc
        });
    }

    private void CreateApiService(ICluster cluster)
    {
        var environmentName = this.GetEnvironmentName();
        var service = new ApplicationLoadBalancedFargateService(this, "ApiService",
            new ApplicationLoadBalancedFargateServiceProps
            {
                Cluster = cluster,
                ServiceName = $"SimplePhotoService-{environmentName}-Api",
                TaskImageOptions = new ApplicationLoadBalancedTaskImageOptions
                {
                    Image = ContainerImage.FromAsset(".",
                        new AssetImageProps { File = "src/SimplePhotoService.Api/Dockerfile" }),
                    Environment = new Dictionary<string, string>
                    {
                        { "AWS__Resources__Table__TableName", _table.TableName },
                        { "AWS__Resources__Bucket__BucketName", _bucket.BucketName },
                        { "AWS__Resources__Cognito__UserPoolId", _userPool.UserPoolId }
                    },
                    ContainerPort = 8080
                },
                RuntimePlatform = new RuntimePlatform
                {
                    CpuArchitecture = CpuArchitecture.ARM64
                },
                PublicLoadBalancer = true,
                MemoryLimitMiB = 1024,
                Cpu = 512,
                DesiredCount = 1,
                ListenerPort = 80
            });
        service.TargetGroup.ConfigureHealthCheck(new HealthCheck { Path = "/health"});
        _bucket.GrantReadWrite(service.TaskDefinition.TaskRole);
        _table.GrantReadWriteData(service.TaskDefinition.TaskRole);
    }

    private void CreateControllerService(ICluster cluster, IBucket bucket)
    {
        var queue = new Queue(this, "BucketNotificationsQueue");
        bucket.AddObjectCreatedNotification(new SqsDestination(queue), new NotificationKeyFilter { Prefix = "upload/"});
        var environmentName = this.GetEnvironmentName();
        var service = new QueueProcessingFargateService(this, "ControllerService",
            new QueueProcessingFargateServiceProps()
            {
                Cluster = cluster,
                ServiceName = $"SimplePhotoService-{environmentName}-Controller",
                Image = ContainerImage.FromAsset(".", new AssetImageProps { File  = "src/SimplePhotoService.Controller/Dockerfile"}),
                Environment = new Dictionary<string, string>() {
                    { "AWS__Resources__Table__TableName", _table.TableName },
                    { "AWS__Resources__Bucket__BucketName", _bucket.BucketName },
                    { "AWS__Resources__Queue__QueueUrl", queue.QueueUrl },
                },
                MemoryLimitMiB = 1024,
                Cpu = 512,
                Queue = queue,
                RuntimePlatform = new RuntimePlatform
                {
                    CpuArchitecture = CpuArchitecture.ARM64
                }
            });
        _table.GrantReadWriteData(service.TaskDefinition.TaskRole);
        _bucket.GrantReadWrite(service.TaskDefinition.TaskRole);
        queue.GrantConsumeMessages(service.TaskDefinition.TaskRole);
        service.TaskDefinition.TaskRole.AddManagedPolicy(ManagedPolicy.FromAwsManagedPolicyName("AmazonBedrockFullAccess"));
        service.TaskDefinition.TaskRole.AddManagedPolicy(ManagedPolicy.FromAwsManagedPolicyName("AmazonRekognitionFullAccess"));
    }
    
    private Table CreateTable()
    {
        var environmentName = this.GetEnvironmentName();
        var table = new Table(this, "AlbumsTable", new TableProps
        {
            TableName = $"SimplePhotoService-{environmentName}-Albums",
            PartitionKey = new Attribute { Name = "PK", Type = AttributeType.STRING },
            SortKey = new Attribute { Name = "SK", Type = AttributeType.STRING },
            BillingMode = BillingMode.PAY_PER_REQUEST,
            RemovalPolicy = this.IsDevelopment() ? RemovalPolicy.DESTROY : RemovalPolicy.RETAIN
        });
        table.AddGlobalSecondaryIndex(new GlobalSecondaryIndexProps
        {
            IndexName = "OwnerIndex",
            PartitionKey = new Attribute { Name = "owner", Type = AttributeType.STRING },
            SortKey = new Attribute { Name = "ownerSK", Type = AttributeType.STRING },
            ProjectionType = ProjectionType.ALL
        });
        return table;
    }

    private Bucket CreateBucket()
    {
        return new Bucket(this, "ContentBucket", new BucketProps
        {
            RemovalPolicy = this.IsDevelopment() ? RemovalPolicy.DESTROY : RemovalPolicy.RETAIN,
            AutoDeleteObjects = this.IsDevelopment()
        });
    }
    
    private (UserPool userPool, IEnumerable<IUserPoolClient> userPoolClients ) CreateUserPool()
    {
        var environmentName = this.GetEnvironmentName();
        var userPool = new UserPool(this, "UserPool", new UserPoolProps
        {
            UserPoolName = $"SimplePhotoService-{environmentName}",
            SignInAliases = new SignInAliases { Email = true, Username = false },
            SelfSignUpEnabled = true,
            RemovalPolicy = this.IsDevelopment() ? RemovalPolicy.DESTROY : RemovalPolicy.RETAIN
        });
        var webClient = userPool.AddClient("WebClient");
        new CfnOutput(this, "AWSUserPoolsUserPoolId", new CfnOutputProps { Value = userPool.UserPoolId });
        new CfnOutput(this, "AWSUserPoolsWebClientId", new CfnOutputProps { Value = webClient.UserPoolClientId });
        return (userPool, new [] { webClient });
    }
}