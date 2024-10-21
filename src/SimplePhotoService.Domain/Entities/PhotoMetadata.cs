namespace SimplePhotoService.Domain.Entities;

public class PhotoMetadata
{
    public int Width { get; set; }
    
    public int Height { get; set; }

    public Label[] Labels { get; set; } = [];

    public bool ExplicitContent { get; set; } = false;
}