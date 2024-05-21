using System.Text.Json.Serialization;

namespace SimplePhotoService.Domain.Entities;

//[JsonDerivedType(typeof(ThumbnailData),0)]
//[JsonDerivedType(typeof(ThumbnailReference), 1)]
public class Thumbnail
{
    public int Width { get; set; }
    
    public int Height { get; set; }
}