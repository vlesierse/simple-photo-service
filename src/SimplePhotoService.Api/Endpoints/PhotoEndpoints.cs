using Amazon.S3;
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
    
    public static async Task<IResult> ListPhotos(Guid albumId, IMediator mediator, IObjectStore objectStore)
    {
        var photos = await mediator.Send(new ListPhotosQuery(albumId));
        // Pre-Sign Urls
        foreach (var photo in photos)
        {
            photo.Url = objectStore.GeneratePreSignUrl(photo.Url[5..].Split('/')[1..].JoinAsString('/'), TimeSpan.FromMinutes(10));
            foreach (var thumbnail in photo.Thumbnails)
            {
                thumbnail.Value.Url = objectStore.GeneratePreSignUrl(thumbnail.Value.Url[5..].Split('/')[1..].JoinAsString('/'), TimeSpan.FromMinutes(10));
            }
        }
        return Results.Ok(photos);
    }
    public static IResult UploadPhotos(Guid albumId, IObjectStore objectStore)
    {
        var id = Guid.NewGuid();
        var url = objectStore.GeneratePreSignUrl($"upload/{albumId}/{id}", TimeSpan.FromMinutes(10), HttpVerb.PUT);
        return Results.Ok(new PhotoUpload(id, url));
    }
}