using System.ComponentModel.DataAnnotations;

namespace SimplePhotoService.Api.Contracts;

public class UploadPhotoInput
{
    public int Count { get; set; } = 1;
}