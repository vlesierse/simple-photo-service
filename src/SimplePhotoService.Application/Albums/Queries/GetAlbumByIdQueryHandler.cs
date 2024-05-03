using Mediator;
using SimplePhotoService.Domain.Entities;
using XaasKit.Domain.Repositories;

namespace SimplePhotoService.Application.Queries;

public class GetAlbumByIdQueryHandler(IReadOnlyRepository<Album, Guid> repository) : IRequestHandler<GetAlbumByIdQuery, Album?>
{
    public async ValueTask<Album?> Handle(GetAlbumByIdQuery request, CancellationToken cancellationToken)
    {
        var album = await repository.FindAsync(request.Id, cancellationToken);
        return album;
    }
}