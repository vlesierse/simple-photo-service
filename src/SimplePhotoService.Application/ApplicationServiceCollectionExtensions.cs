using Microsoft.Extensions.DependencyInjection;

namespace SimplePhotoService.Application;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        _ = services.AddMediator(options => options.Namespace = "SimplePhotoService.Application");
        return services;
    }
}