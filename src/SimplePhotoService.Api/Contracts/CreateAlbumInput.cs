using System.ComponentModel.DataAnnotations;

namespace SimplePhotoService.Api.Contracts;

public class CreateAlbumInput
{
    [Required]
    public required string Title { get; set; }
}