using Mediator;
using SimplePhotoService.Domain.Entities;
using SimplePhotoService.Domain.Repositories;

namespace SimplePhotoService.Application.Commands;

public class CreateAlbumCommandHandlers(IAlbumRepository repository)
    : ICommandHandler<CreateAlbumCommand, CreateAlbumResult>
{
    public async ValueTask<CreateAlbumResult> Handle(CreateAlbumCommand command, CancellationToken cancellationToken)
    {
        var album = new Album() { Title = command.Title, OwnerId = command.OwnerId };
        _ = await repository.InsertAsync(album, cancellationToken);
        return new CreateAlbumResult(album.Id, album.Title);
    }
}