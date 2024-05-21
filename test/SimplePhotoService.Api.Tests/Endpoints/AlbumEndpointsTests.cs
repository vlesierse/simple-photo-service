using System.Security.Claims;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;
using SimplePhotoService.Api.Contracts;
using SimplePhotoService.Api.Routes;
using SimplePhotoService.Application.Commands;

namespace SimplePhotoService.Api.Tests.Endpoints;

public class AlbumEndpointsTests
{
    private readonly IMediator _mediator;

    public AlbumEndpointsTests()
    {
        _mediator = Substitute.For<IMediator>();
    }
    
    [Fact]
    public async Task Handle_CreateAlbumRequest()
    {
        // Arrange
        var input = new CreateAlbumInput { Title = "Album" };
        var principal = new ClaimsPrincipal();
        principal.AddIdentity(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, "testuser") }));
        _mediator.Send(Arg.Any<CreateAlbumCommand>()).ReturnsForAnyArgs(new CreateAlbumResult(Guid.Empty, "Album"));
        
        // Act
        var result = await AlbumEndpoints.CreateAlbum(input, _mediator, principal);
        
        // Assert
        Assert.IsType<Ok<Album>>(result);
        var okResult = (Ok<Album>)result;
        Assert.NotNull(okResult.Value);
        Assert.Equal("Album", okResult.Value.Title);
    }
}