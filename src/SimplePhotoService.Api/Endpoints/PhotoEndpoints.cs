using Mediator;
using SimplePhotoService.Api.Contracts;
using SimplePhotoService.Application.Photos.Queries;
using SimplePhotoService.Infrastructure.Storage;

namespace SimplePhotoService.Api.Routes;

public static class PhotoEndpoints
{
    public static WebApplication MapPhotoEndpoints(this WebApplication app)
    {
        var root = app
            .MapGroup("/albums/{albumId:guid}/photos")
            .RequireAuthorization();

        _ = root.MapGet("/", ListPhotos);
        _ = root.MapPost("/", UploadPhotos);
        
        return app;
    }
    
    public static async Task<IResult> ListPhotos(Guid albumId, IMediator mediator)
    {
        var photos = await mediator.Send(new ListPhotosQuery(albumId));
        return Results.Ok(photos);
    }
    public static IResult UploadPhotos(Guid albumId, IObjectStore objectStore)
    {
        var id = Guid.NewGuid();
        var url = objectStore.GeneratePreSignUrl($"upload/{albumId}/{id}", TimeSpan.FromMinutes(10));
        return Results.Ok(new PhotoUpload(id, url));
    }
}