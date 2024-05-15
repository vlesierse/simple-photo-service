using Mediator;
using SimplePhotoService.Domain.Entities;
using SimplePhotoService.Domain.Repositories;

namespace SimplePhotoService.Application.Queries;

public class ListAlbumsQueryHandler(IAlbumRepository repository) : IRequestHandler<ListAlbumsQuery, IList<Album>>
{
    public async ValueTask<IList<Album>> Handle(ListAlbumsQuery query, CancellationToken cancellationToken)
    {
        var result = await repository.ListAblumsForOwner(query.OwnerId, cancellationToken);
        return result;
    }
}