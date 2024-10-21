using System.Net.Mime;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using Image = SixLabors.ImageSharp.Image;

namespace SimplePhotoService.Controller.Processing;

public class SmartCropProcessor(IOptions<SmartCropOptions> options) : IImageProcessor
{
    public SmartCropOptions Options { get; } = options.Value;

    public int Priority { get; } = 10;

    public Task ProcessImageAsync(ImageProcessorContext context, CancellationToken cancellationToken)
    {
        var resizeOptions = new ResizeOptions
        {
            Size = new Size(1024),
            Mode = ResizeMode.Max
        };
        var label = context.Metadata.Labels.OrderByDescending(l => l.Confidence).FirstOrDefault(x => (x.Name == Options.CropLabel || x.Parents.Contains(Options.CropLabel)) && x.BoundingBox != null);
        var croppedImage = context.OriginalImage.Clone(x =>
        {
            
            if (label?.BoundingBox != null)
            {
                x.Crop(new Rectangle(
                    (int)(context.OriginalImage.Width * label.BoundingBox.Left) - Options.Padding,
                    (int)(context.OriginalImage.Height * label.BoundingBox.Top),
                    (int)(context.OriginalImage.Width * label.BoundingBox.Width),
                    (int)(context.OriginalImage.Height * label.BoundingBox.Height)
                    ));
            }
            x.Resize(resizeOptions);
        });
        context.Image = croppedImage;
        return Task.CompletedTask;
    }
}