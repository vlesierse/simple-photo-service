using Constructs;
using SimplePhotoService.CDK.Extensions;
using Stack = Amazon.CDK.Stack;

namespace SimplePhotoService.CDK;

public class ApplicationStackProps : StackProps;

public interface IApplicationStack;

public class ApplicationStack : Stack, IApplicationStack
{
    private readonly Table _table;
    private readonly HttpApi _api;
    private readonly UserPool _userPool;
    private readonly IEnumerable<IUserPoolClient> _userPoolWebClients;
    
    public string TableName { get; init; }
    
    public ApplicationStack(Construct scope, string id, ApplicationStackProps props)
        : base(scope, id, props)
    {
        _table = CreateTable();
        (_userPool, _userPoolWebClients) = CreateUserPool();
        _api = CreateApi();
        // Expose Stack properties for external referencing
        TableName = _table.TableName;
    }

    private HttpApi CreateApi()
    {
        var environmentName = this.GetEnvironmentName();
        var apiFunction = new DotNetFunction(this, "ApiFunction", new DotNetFunctionProps
        {
            ProjectDir = "src/SimplePhotoService.Api",
            Runtime = Runtime.DOTNET_8,
            Environment = new Dictionary<string, string>() {
                { "SimplePhotoService__AmazonDynamoDB__TableNamePrefix", $"SimplePhotoService-{environmentName}-" }
            },
            FunctionName = $"SimplePhotoService-{environmentName}-Api",
            Timeout = Duration.Seconds(10),
            LogRetention = this.IsDevelopment() ? RetentionDays.ONE_DAY : RetentionDays.ONE_MONTH
        });
        var api = new HttpApi(this, "Api", new HttpApiProps
        {
            ApiName = $"SimplePhotoService-{environmentName}",
            DefaultIntegration = new HttpLambdaIntegration("ApiIntegration", apiFunction),
            DefaultAuthorizer = new HttpUserPoolAuthorizer("ApiAuthorizer", _userPool, new HttpUserPoolAuthorizerProps
            {
                UserPoolClients = _userPoolWebClients.ToArray()
            }),
            CorsPreflight = new CorsPreflightOptions
            {
                AllowHeaders = ["Authorization"],
                AllowMethods = [CorsHttpMethod.ANY] ,
                AllowOrigins = ["*"],
                //MaxAge = Duration.Minutes(10)
            }
        });
        api.AddCorsPreflightRoute();
        _table.GrantReadWriteData(apiFunction);
        _ = new CfnOutput(this, "AWSApiEndpointUrl", new CfnOutputProps { Value = api.Url ?? api.ApiEndpoint });
        return api;
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
        _ = new CfnOutput(this, "AWSUserPoolsUserPoolId", new CfnOutputProps { Value = userPool.UserPoolId });
        _ = new CfnOutput(this, "AWSUserPoolsWebClientId", new CfnOutputProps { Value = webClient.UserPoolClientId });
        return (userPool, new [] { webClient });
    }
}