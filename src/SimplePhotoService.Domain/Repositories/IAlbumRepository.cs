using SimplePhotoService.Domain.Entities;
using XaasKit.Domain.Repositories;

namespace SimplePhotoService.Domain.Repositories;

public interface IAlbumRepository : IRepository<Album, Guid>
{
    Task<IList<Album>> ListAblumsForOwner(string ownerId, CancellationToken cancellationToken = default);
}