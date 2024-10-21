using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using Label = SimplePhotoService.Domain.Entities.Label;
using RekognitionLabel = Amazon.Rekognition.Model.Label;
using BoundingBox = SimplePhotoService.Domain.Entities.BoundingBox;

namespace SimplePhotoService.Controller.Processing;

public class LabelDetectionProcessor(IAmazonRekognition rekognition, IOptions<LabelDetectionOptions> options, ILogger<LabelDetectionProcessor> logger) : IImageProcessor
{

    public LabelDetectionOptions Options { get; } = options.Value;

    public int Priority { get; } = 100;

    public async Task ProcessImageAsync(ImageProcessorContext context, CancellationToken cancellationToken)
    {
        using var imageStream = new MemoryStream();
        await context.OriginalImage.SaveAsPngAsync(imageStream, cancellationToken);
        var response = await rekognition.DetectLabelsAsync(new DetectLabelsRequest()
        {
            Image = new Amazon.Rekognition.Model.Image { Bytes = imageStream }
        }, cancellationToken);
        logger.LogInformation("Detected label: {Labels}", response.Labels.Select(l => $"{l.Name} ({l.Confidence})").JoinAsString(", "));
        context.Metadata.Labels = response.Labels.Where(x => x.Confidence > Options.ConfidenceThreshold).Select(CreateLabel).ToArray();
    }

    private static Label CreateLabel(RekognitionLabel label)
    {
        var result = new Label { Name = label.Name, Confidence = label.Confidence, Parents = label.Parents.Select(x => x.Name).ToArray()};
        var instance = label.Instances.FirstOrDefault();
        if (instance != null)
        {
            result.BoundingBox = new BoundingBox(instance.BoundingBox.Left, instance.BoundingBox.Top, instance.BoundingBox.Width, instance.BoundingBox.Height);
        }
        return result;
    }
    
}