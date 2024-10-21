using System.Text.Json.Serialization;

namespace SimplePhotoService.Domain.Entities;

public class Label
{
    public required string Name { get; set; }

    public required float Confidence { get; set; }
    
    [JsonIgnore]
    public string[] Parents { get; set; } = [];
    
    [JsonIgnore]
    public BoundingBox? BoundingBox { get; set; }
}