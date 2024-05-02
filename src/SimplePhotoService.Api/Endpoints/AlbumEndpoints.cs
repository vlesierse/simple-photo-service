using Mediator;
using Microsoft.AspNetCore.Mvc;
using SimplePhotoService.Api.Contracts;
using SimplePhotoService.Application.Commands;
using SimplePhotoService.Application.Queries;

namespace SimplePhotoService.Api.Routes;

public static class AlbumEndpoints
{
    public static WebApplication MapAlbumEndpoints(this WebApplication app)
    {
        var root = app.MapGroup("/albums");
        
        _ = root.MapGet("/{id:guid}", GetAlbumById);
        _ = root.MapGet("/", ListAlbums);
        _ = root.MapPost("/", CreateAlbum);
        
        return app;
    }
    
    public static IResult ListAlbums(IMediator mediator)
    {
        var albums = mediator.Send(new ListAlbumsQuery());
        return Results.Ok(albums);
    }
    
    public static IResult GetAlbumById(Guid id)
    {
        return Results.Ok(new Album(id, ""));
    }
    
    public static async Task<IResult> CreateAlbum([FromBody] CreateAlbumInput input, IMediator mediator)
    {
        var result = await mediator.Send(new CreateAlbumCommand(input.Title!));
        return Results.Ok(result.FromResult());
    }
}