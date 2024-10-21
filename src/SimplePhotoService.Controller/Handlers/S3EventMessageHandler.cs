using System.Text.Json;
using Amazon.SQS.Model;
using Microsoft.Extensions.Logging;
using SimplePhotoService.Controller.Processing;
using SimplePhotoService.Domain.Entities;
using SimplePhotoService.Domain.Repositories;
using SimplePhotoService.Infrastructure.Storage;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace SimplePhotoService.Controller.Handlers;

public class S3EventMessageHandler(
    IObjectStore objectStore,
    IPhotoRepository photoRepository,
    IEnumerable<IImageProcessor> imageProcessors,
    ILogger<S3EventMessageHandler> logger) : IMessageHandler
{

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
    
    public async Task Handle(Message message, CancellationToken cancellationToken = default)
    {
        var s3Event = JsonSerializer.Deserialize<S3EventMessage>(message.Body, SerializerOptions) ?? S3EventMessage.Empty;
        foreach (var record in s3Event.Records)
        {
            await using var stream = await objectStore.GetObjectStreamAsync(record.S3.Object.Key);
            if (stream == null)
            {
                logger.LogWarning("Object with key {Key} not found! Skipping...", record.S3.Object.Key);
                continue;
            }

            using var image = await Image.LoadAsync(stream, cancellationToken);

            var keySegments = record.S3.Object.Key.Split('/');
            var albumId = keySegments[1];
            var photoId = keySegments[2];
            var photoKey = $"album/{albumId}/{photoId}";
            var photoUrl = $"s3://{record.S3.Bucket.Name}/{photoKey}";
            
            // Copy object from upload prefix to album prefix.
            await CopyOriginal(record.S3.Object.Key, photoKey + "_original");
            
            using var context = new ImageProcessorContext(image);
            // Process the image
            var processors = imageProcessors.GroupBy(p => p.Priority).OrderByDescending(g => g.Key);
            foreach (var processor in processors)
            {
                await Task.WhenAll(processor.Select(p => p.ProcessImageAsync(context, cancellationToken)).ToArray());
            }
            
            var photo = new Photo(Guid.Parse(photoId), Guid.Parse(albumId))
            {
                Url = photoUrl,
                Metadata = context.Metadata
            };

            await Task.WhenAll(
                UploadImageAsync(context.Image, photoKey),
                photoRepository.InsertAsync(photo, cancellationToken)
            );
            
            logger.LogInformation("Uploaded {Key}", record.S3.Object.Key);
        }
    }

    private Task<bool> CopyOriginal(string sourceKey, string destinationKey)
    {
        return objectStore.CopyObjectAsync(sourceKey, destinationKey);
    }
    
    private async Task<bool> UploadImageAsync(Image image, string objectKey)
    {
        using var stream = new MemoryStream();
        await image.SaveAsJpegAsync(stream);
        stream.Position = 0;
        return await objectStore.PutObjectStreamAsync(objectKey, stream, "image/jpeg");
    }
    
}