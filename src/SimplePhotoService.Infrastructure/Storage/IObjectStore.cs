using Amazon.S3;

namespace SimplePhotoService.Infrastructure.Storage;

public interface IObjectStore
{
    Task<bool> CopyObjectAsync(string sourceKey, string destinationKey);
    
    Task<Stream?> GetObjectStreamAsync(string objectKey);
    
    Task<bool> PutObjectStreamAsync(string objectKey, Stream stream, string? contentType = default);
    
    string GeneratePreSignUrl(string objectKey, TimeSpan duration, HttpVerb verb = HttpVerb.GET);
}