using Amazon.DynamoDBv2;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using XaasKit.Amazon.DynamoDB.Repositories;
using XaasKit.Domain.Repositories;

namespace XaasKit.Amazon.DynamoDB;

public static class DynamoDBServiceCollectionExtensions
{
    public static IServiceCollection AddDynamoDB(this IServiceCollection services, IConfiguration configuration)
    {
        _ = services.AddAWSService<IAmazonDynamoDB>();
        // DynamoDB Client
        _ = services.AddOptions().Configure<DynamoDBOptions>(configuration);
        // Repositories
        services.TryAddTransient(typeof(IRepository<>), typeof(DynamoDbRepository<>));
        services.TryAddTransient(typeof(IRepository<,>), typeof(DynamoDbRepository<,>));
        services.TryAddTransient(typeof(IReadOnlyRepository<>), typeof(DynamoDbRepository<>));
        services.TryAddTransient(typeof(IReadOnlyRepository<,>), typeof(DynamoDbRepository<,>));
        return services;
    }
}