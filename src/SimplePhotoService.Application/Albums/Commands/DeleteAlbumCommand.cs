using Mediator;

namespace SimplePhotoService.Application.Commands;

public record DeleteAlbumCommand(Guid Id) : ICommand<bool>;