using Mediator;
using SimplePhotoService.Application.Photos.Queries;

namespace SimplePhotoService.Api.Routes;

public static class PhotoEndpoints
{
    public static WebApplication MapPhotoEndpoints(this WebApplication app)
    {
        var root = app
            .MapGroup("/albums/{albumId:guid}/photos")
            .RequireAuthorization();

        _ = root.MapGet("/", ListPhotos);
        
        return app;
    }
    
    public static async Task<IResult> ListPhotos(Guid albumId, IMediator mediator)
    {
        var photos = await mediator.Send(new ListPhotosQuery(albumId));
        return Results.Ok(photos);
    }
    
}