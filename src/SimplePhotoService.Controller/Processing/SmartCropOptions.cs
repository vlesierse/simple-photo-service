namespace SimplePhotoService.Controller.Processing;

public class SmartCropOptions
{
    public int Size { get; set; } = 1024;

    public int Padding { get; set; } = 10;
    
    public string? CropLabel { get; set; }
}