using System.Text.Json.Serialization;
using XaasKit.Domain.Entities;

namespace SimplePhotoService.Domain.Entities;

public sealed class Album : Entity<Guid>
{
    public Album() => Id = Guid.NewGuid();

    [JsonConstructor]
    public Album(Guid id) => Id = id;

    public string? Title { get; set; }

    public string? OwnerId { get; set; }
}