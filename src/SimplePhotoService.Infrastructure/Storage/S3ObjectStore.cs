using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;

namespace SimplePhotoService.Infrastructure.Storage;

public class S3ObjectStore(IAmazonS3 s3Client, IOptions<S3ObjectStoreOptions> options) : IObjectStore
{

    public S3ObjectStoreOptions Options { get; } = options.Value;
    public string GeneratePreSignUrl(
        string objectKey,
        TimeSpan duration)
    {
        var request = new GetPreSignedUrlRequest()
        {
            BucketName = Options.BucketName,
            Key = objectKey,
            Verb = HttpVerb.PUT,
            Expires = DateTime.UtcNow.Add(duration),
        };
        return s3Client.GetPreSignedURL(request);
    }
}