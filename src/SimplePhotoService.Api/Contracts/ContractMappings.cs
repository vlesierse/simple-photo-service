using SimplePhotoService.Application.Commands;

namespace SimplePhotoService.Api.Contracts;

public static class ContractMappings
{
    public static Album FromResult(this CreateAlbumResult result)
    {
        return new Album(result.Id, result.Title);
    }
}