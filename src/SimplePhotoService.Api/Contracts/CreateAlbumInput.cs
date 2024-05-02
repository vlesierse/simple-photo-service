using System.ComponentModel.DataAnnotations;

namespace SimplePhotoService.Api.Contracts;

public class CreateAlbumInput
{
    [Required]
    public string? Title { get; set; }
}