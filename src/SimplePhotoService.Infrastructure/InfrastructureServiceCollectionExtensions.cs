using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using XaasKit.Amazon.DynamoDB;

namespace SimplePhotoService.Infrastructure;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Add DynamoDB Storage support.
        _ = services.AddDynamoDB(configuration.GetSection("DynamoDB"));
        return services;
    }
}