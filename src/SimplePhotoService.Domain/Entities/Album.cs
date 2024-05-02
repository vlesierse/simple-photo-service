using XaasKit.Domain.Entities;

namespace SimplePhotoService.Domain.Entities;

public sealed class Album : Entity<Guid>
{
    public Album() => Id = Guid.NewGuid();

    public string? Title { get; set; }

    public string? OwnerId { get; set; }
}