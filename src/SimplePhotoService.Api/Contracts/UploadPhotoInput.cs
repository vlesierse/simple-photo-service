using System.ComponentModel.DataAnnotations;

namespace SimplePhotoService.Api.Contracts;

public class UploadPhotoInput
{
    [Required]
    public Guid? AlbumId { get; set; }
}