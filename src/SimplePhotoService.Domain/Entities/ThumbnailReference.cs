namespace SimplePhotoService.Domain.Entities;

public class ThumbnailReference : Thumbnail
{
    public required string Url { get; set; }
}