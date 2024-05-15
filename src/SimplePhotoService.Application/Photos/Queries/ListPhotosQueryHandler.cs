using Mediator;
using SimplePhotoService.Application.Photos.Queries;
using SimplePhotoService.Domain.Entities;
using SimplePhotoService.Domain.Repositories;
using XaasKit.Domain.Repositories;

namespace SimplePhotoService.Application.Queries;

public class ListPhotosQueryHandler(IPhotoRepository repository) : IRequestHandler<ListPhotosQuery, IList<Photo>>
{
    public IReadOnlyRepository<Photo> Repository { get; } = repository;

    public async ValueTask<IList<Photo>> Handle(ListPhotosQuery query, CancellationToken cancellationToken)
    {
        var result = await repository.ListPhotosInAlbum(query.AlbumId, cancellationToken);
        return result;
    }
}