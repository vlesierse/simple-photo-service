using Mediator;
using SimplePhotoService.Domain.Entities;

namespace SimplePhotoService.Application.Photos.Queries;

public record ListPhotosQuery(Guid AlbumId) : IRequest<IList<Photo>>;