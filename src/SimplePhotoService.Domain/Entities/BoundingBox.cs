namespace SimplePhotoService.Domain.Entities;

public class BoundingBox(float x, float y, float width, float height)
{
    public float Left { get; set; } = x;
    
    public float Top { get; set; } = y;
    
    public float Width { get; set; } = width;
    
    public float Height { get; set; } = height;
}