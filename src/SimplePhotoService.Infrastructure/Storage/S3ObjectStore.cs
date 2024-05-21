using System.Net;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;

namespace SimplePhotoService.Infrastructure.Storage;

public class S3ObjectStore(IAmazonS3 s3Client, IOptions<S3ObjectStoreOptions> options) : IObjectStore
{
    public S3ObjectStoreOptions Options { get; } = options.Value;
    
    public async Task<Stream?> GetObjectStreamAsync(string objectKey)
    {
        var request = new GetObjectRequest
        {
            BucketName = Options.BucketName,
            Key = objectKey
        };
        var result = await s3Client.GetObjectAsync(request);
        return result.HttpStatusCode != HttpStatusCode.OK ? null : result.ResponseStream;
    }

    public async Task<bool> PutObjectStreamAsync(string objectKey, Stream stream, string? contentType = default)
    {
        var request = new PutObjectRequest
        {
            BucketName = Options.BucketName,
            Key = objectKey,
            InputStream = stream,
            ContentType = contentType
        };
        var result = await s3Client.PutObjectAsync(request);
        return result.HttpStatusCode == HttpStatusCode.OK;
    }

    public async Task<bool> CopyObjectAsync(string sourceKey, string destinationKey)
    {
        var request = new CopyObjectRequest()
        {
            SourceBucket = Options.BucketName,
            DestinationBucket = Options.BucketName,
            SourceKey = sourceKey,
            DestinationKey = destinationKey
        };
        var result = await s3Client.CopyObjectAsync(request);
        return result.HttpStatusCode == HttpStatusCode.OK;
    }

    public string GeneratePreSignUrl(
        string objectKey,
        TimeSpan duration, HttpVerb verb = HttpVerb.GET)
    {
        var request = new GetPreSignedUrlRequest()
        {
            BucketName = Options.BucketName,
            Key = objectKey,
            Verb = verb,
            Expires = DateTime.UtcNow.Add(duration),
        };
        return s3Client.GetPreSignedURL(request);
    }
}