namespace SimplePhotoService.Infrastructure.Storage;

public interface IObjectStore
{
    public string GeneratePreSignUrl(string objectKey, TimeSpan duration);
}