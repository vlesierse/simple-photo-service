using Amazon.Runtime.SharedInterfaces;
using Amazon.S3;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimplePhotoService.Domain.Repositories;
using SimplePhotoService.Infrastructure.Domain;
using SimplePhotoService.Infrastructure.Storage;
using XaasKit.Amazon.DynamoDB;

namespace SimplePhotoService.Infrastructure;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Add DynamoDB Storage support.
        _ = services.AddDynamoDB(configuration.GetSection("AWS:Resources:Table"));
        // Add S3 Object Store
        _ = services.AddAWSService<IAmazonS3>();
        _ = services.AddOptions().Configure<S3ObjectStoreOptions>(configuration.GetSection("AWS:Resources:Bucket"));
        _ = services.AddTransient<IObjectStore, S3ObjectStore>();
        // Add Repositories
        _ = services
            .AddTransient<IAlbumRepository, AlbumRepository>()
            .AddTransient<IPhotoRepository, PhotoRepository>();
        return services;
    }
}