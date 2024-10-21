namespace SimplePhotoService.Controller.Processing;

public interface IImageProcessor
{
    int Priority { get; }
    Task  ProcessImageAsync(ImageProcessorContext context, CancellationToken cancellationToken);
}