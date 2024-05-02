using Mediator;
using SimplePhotoService.Domain.Entities;
using XaasKit.Domain.Repositories;

namespace SimplePhotoService.Application.Commands;

public class CreateAlbumCommandHandlers(IRepository<Album> albumRepository)
    : ICommandHandler<CreateAlbumCommand, CreateAlbumResult>
{
    public async ValueTask<CreateAlbumResult> Handle(CreateAlbumCommand command, CancellationToken cancellationToken)
    {
        var album = new Album() { Title = command.Title };
        _ = await albumRepository.InsertAsync(album, cancellationToken);
        return new CreateAlbumResult(album.Id, album.Title);
    }
}