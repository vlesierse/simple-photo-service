using Mediator;
using SimplePhotoService.Domain.Entities;

namespace SimplePhotoService.Application.Queries;

public record ListAlbumsQuery : IRequest<List<Album>>;