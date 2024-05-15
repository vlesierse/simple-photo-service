using SimplePhotoService.Domain.Entities;
using XaasKit.Domain.Repositories;

namespace SimplePhotoService.Domain.Repositories;

public interface IPhotoRepository : IRepository<Photo, Guid>
{
    Task<IList<Photo>> ListPhotosInAlbum(Guid albumId, CancellationToken cancellationToken = default);
}