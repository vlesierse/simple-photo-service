using Amazon.CDK.AWS.Lambda;
using HttpMethod = Amazon.CDK.AWS.Apigatewayv2.HttpMethod;

namespace SimplePhotoService.CDK.Extensions;

public static class HttpApiExtensions
{
    public static void AddCorsPreflightRoute(this HttpApi api)
    {
        var function = new Function(api, "CORSPreflightFunction", new FunctionProps
        {
            Runtime = Runtime.NODEJS_18_X,
            Architecture = Architecture.ARM_64,
            Code = Code.FromInline("exports.handler = async () => { return { statusCode: 204 } }"),
            Handler = "index.handler"
        });
        api.AddRoutes(new AddRoutesOptions
        {
            Path = "/{proxy+}",
            Methods = [HttpMethod.OPTIONS],
            Integration = new HttpLambdaIntegration("CORSPreflight", function),
            Authorizer = new HttpNoneAuthorizer()
        });
    }
}