using SimplePhotoService.Domain.Entities;
using SixLabors.ImageSharp;

namespace SimplePhotoService.Controller.Processing;

public class ImageProcessorContext(Image image) : IDisposable
{
    private Image? _image;

    public Image Image
    {
        get => _image ?? OriginalImage;
        set
        {
            _image = value;
            Metadata.Width = value.Width;
            Metadata.Height = value.Height;
        }
    }
    
    public Image OriginalImage { get; } = image;

    public  PhotoMetadata Metadata { get; } = new() { Width = image.Width, Height = image.Height };

    void IDisposable.Dispose()
    {
        _image?.Dispose();
    }
}