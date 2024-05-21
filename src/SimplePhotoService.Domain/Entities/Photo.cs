using System.Text.Json.Serialization;
using XaasKit.Domain.Entities;

namespace SimplePhotoService.Domain.Entities;

public sealed class Photo : Entity<Guid>
{
    public Photo() => Id = Guid.NewGuid();

    public Photo(Guid id, Guid albumId) : this(id)
    {
        AlbumId = albumId;
    }

    [JsonConstructor]
    public Photo(Guid id) => Id = id;

    public Guid AlbumId { get; set; }
    
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    
    public required string Url { get; set; }

    public required PhotoMetadata Metadata { get; set; } = new PhotoMetadata();

    public IDictionary<string, ThumbnailReference> Thumbnails { get; set; } = new Dictionary<string, ThumbnailReference>();

    public void AddThumbnail(string name, ThumbnailReference thumbnail)
    {
        Thumbnails.TryAdd(name, thumbnail);
    }
}