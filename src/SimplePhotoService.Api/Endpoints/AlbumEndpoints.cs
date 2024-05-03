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
        
        _ = root.MapGet("/", ListAlbums);
        _ = root.MapPost("/", CreateAlbum);
        _ = root.MapGet("/{id:guid}", GetAlbumById);
        _ = root.MapDelete("/{id:guid}", DeleteAlbum);
        
        return app;
    }
    
    public static async Task<IResult> ListAlbums(IMediator mediator)
    {
        var albums = await mediator.Send(new ListAlbumsQuery());
        return Results.Ok(albums);
    }
    
    public static async Task<IResult> DeleteAlbum(Guid id, IMediator mediator)
    {
        await mediator.Send(new DeleteAlbumCommand(id));
        return Results.Accepted();
    }
    
    public static async Task<IResult> GetAlbumById(Guid id, IMediator mediator)
    {
        var result = await mediator.Send(new GetAlbumByIdQuery(id));
        return result == null ? Results.NotFound() : Results.Ok(result);
    }
    
    public static async Task<IResult> CreateAlbum([FromBody] CreateAlbumInput input, IMediator mediator)
    {
        var result = await mediator.Send(new CreateAlbumCommand(input.Title!));
        return Results.Ok(result.FromResult());
    }
}