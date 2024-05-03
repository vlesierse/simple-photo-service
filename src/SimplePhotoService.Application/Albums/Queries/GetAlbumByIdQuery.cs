using Mediator;
using SimplePhotoService.Domain.Entities;

namespace SimplePhotoService.Application.Queries;

public record GetAlbumByIdQuery(Guid Id) : IRequest<Album?>;