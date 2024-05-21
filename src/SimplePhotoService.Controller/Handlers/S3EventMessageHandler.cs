using System.Text.Json;
using Amazon.SQS.Model;
using Microsoft.Extensions.Logging;
using SimplePhotoService.Domain.Entities;
using SimplePhotoService.Domain.Repositories;
using SimplePhotoService.Infrastructure.Storage;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Transforms;

namespace SimplePhotoService.Controller.Handlers;

public class S3EventMessageHandler(IObjectStore objectStore, IPhotoRepository photoRepository, ILogger<S3EventMessageHandler> logger) : IMessageHandler
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
            await CopyOriginal(record.S3.Object.Key, photoKey);
            
            // Generate Thumbnails
            //using var smallThumbnail = await GenerateThumbnailAsync(image, 40);
            await UploadThumbnailAsync(image, 160, $"{photoKey}_medium");
            await UploadThumbnailAsync(image, 1536, $"{photoKey}_large");
            
            var photo = new Photo(Guid.Parse(photoId), Guid.Parse(albumId))
            {
                Url = photoUrl,
                Metadata = new PhotoMetadata() { Width = image.Width, Height = image.Height }
            };
            //photo.AddThumbnail("small", new ThumbnailData { Data = smallThumbnail.GetBuffer(), Width = 40, Height = 40});
            photo.AddThumbnail("medium", new ThumbnailReference { Url = $"{photoUrl}_medium", Width = 160, Height = 160});
            photo.AddThumbnail("large", new ThumbnailReference { Url = $"{photoUrl}_large", Width = 1536, Height = 1536});
            
            await photoRepository.InsertAsync(photo, cancellationToken);
            
            logger.LogInformation("Uploaded {Key}", record.S3.Object.Key);
        }
    }

    private Task<bool> CopyOriginal(string sourceKey, string destinationKey)
    {
        return objectStore.CopyObjectAsync(sourceKey, destinationKey);
    }
    
    private static async Task<MemoryStream> GenerateThumbnailAsync(Image image, int size)
    {
        var resizeOptions = new ResizeOptions
        {
            Size = new Size(size),
            Mode = ResizeMode.Max
        };
        using var thumbnailImage = image.Clone(x => x.Resize(resizeOptions));
        var memoryStream = new MemoryStream();
        await thumbnailImage.SaveAsWebpAsync(memoryStream);
        memoryStream.Position = 0;
        return memoryStream;
    }
    
    private async Task<bool> UploadThumbnailAsync(Image image, int size, string objectKey)
    {
        using var stream = await GenerateThumbnailAsync(image, size);
        return await objectStore.PutObjectStreamAsync(objectKey, stream, "image/webp");
    }
    
}