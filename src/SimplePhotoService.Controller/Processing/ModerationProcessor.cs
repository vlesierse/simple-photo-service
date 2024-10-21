using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;

namespace SimplePhotoService.Controller.Processing;

public class ModerationProcessor(IAmazonRekognition rekognition, ILogger<ModerationProcessor> logger) : IImageProcessor
{
    public int Priority { get; } = 100;

    public async Task ProcessImageAsync(ImageProcessorContext context, CancellationToken cancellationToken)
    {
        using var imageStream = new MemoryStream();
        await context.OriginalImage.SaveAsPngAsync(imageStream, cancellationToken);
        var response = await rekognition.DetectModerationLabelsAsync(new DetectModerationLabelsRequest()
        {
            Image = new Amazon.Rekognition.Model.Image { Bytes = imageStream }
        }, cancellationToken);
        if (response.ModerationLabels.Any(x => x.Confidence > 50))
        {
            logger.LogInformation("Explicit content detected in image {Labels}", response.ModerationLabels.Select(l => $"{l.Name} :{l.Confidence}").JoinAsString(", "));
            context.Metadata.ExplicitContent = true;
        }
    }
}