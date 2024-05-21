namespace SimplePhotoService.Domain.Entities;

public class ThumbnailData : Thumbnail
{
    public required byte[] Data { get; set; }
}