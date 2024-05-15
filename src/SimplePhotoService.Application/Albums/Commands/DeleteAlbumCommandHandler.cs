using Mediator;
using SimplePhotoService.Domain.Repositories;

namespace SimplePhotoService.Application.Commands;

public class DeleteAlbumCommandHandler(IAlbumRepository repository) : ICommandHandler<DeleteAlbumCommand, bool>
{
    public async ValueTask<bool> Handle(DeleteAlbumCommand command, CancellationToken cancellationToken)
    {
        var result = await repository.DeleteAsync(command.Id, cancellationToken);
        return result;
    }
}