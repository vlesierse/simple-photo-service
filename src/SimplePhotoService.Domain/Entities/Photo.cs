using System.Text.Json.Serialization;
using XaasKit.Domain.Entities;

namespace SimplePhotoService.Domain.Entities;

public sealed class Photo : Entity<Guid>
{
    public Photo() => Id = Guid.NewGuid();

    [JsonConstructor]
    public Photo(Guid id) => Id = id;

    public Guid AlbumId { get; set; }
    
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}