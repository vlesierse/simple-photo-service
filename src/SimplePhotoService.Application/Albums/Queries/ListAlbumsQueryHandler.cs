using Mediator;
using SimplePhotoService.Domain.Entities;
using XaasKit.Domain.Repositories;

namespace SimplePhotoService.Application.Queries;

public class ListAlbumsQueryHandler(IReadOnlyRepository<Album> repository) : IRequestHandler<ListAlbumsQuery, List<Album>>
{
    public async ValueTask<List<Album>> Handle(ListAlbumsQuery request, CancellationToken cancellationToken)
    {
        var result = await repository.GetListAsync(cancellationToken);
        return result;
    }
}