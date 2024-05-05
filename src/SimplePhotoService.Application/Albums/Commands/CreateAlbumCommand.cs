using Mediator;

namespace SimplePhotoService.Application.Commands;

public record CreateAlbumCommand(string Title, string OwnerId) : ICommand<CreateAlbumResult>;

public record CreateAlbumResult(Guid Id, string Title);