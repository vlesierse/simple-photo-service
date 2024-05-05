using Mediator;
using SimplePhotoService.Application.Photos.Queries;
using SimplePhotoService.Domain.Entities;
using XaasKit.Domain.Repositories;

namespace SimplePhotoService.Application.Queries;

public class ListPhotosQueryHandler(IReadOnlyRepository<Photo> repository) : IRequestHandler<ListPhotosQuery, List<Photo>>
{
    public IReadOnlyRepository<Photo> Repository { get; } = repository;

    public ValueTask<List<Photo>> Handle(ListPhotosQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}