namespace SimplePhotoService.Api.Contracts;

public record Photo(Guid Id, Guid AlbumId, string Url);