using Mediator;

namespace SimplePhotoService.Application.Photos.Commands;

public record AddPhotoCommand(Guid AlbumId, string Url): ICommand;