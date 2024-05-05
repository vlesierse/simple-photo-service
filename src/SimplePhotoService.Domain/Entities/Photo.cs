using System.Text.Json.Serialization;
using XaasKit.Domain.Entities;

namespace SimplePhotoService.Domain.Entities;

public sealed class Photo : Entity<Guid>
{
    public Photo() => Id = Guid.NewGuid();

    [JsonConstructor]
    public Photo(Guid id) => Id = id;
}