using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Microsoft.Extensions.Options;
using SimplePhotoService.Domain.Entities;
using SimplePhotoService.Domain.Repositories;
using XaasKit.Amazon.DynamoDB.Repositories;

namespace SimplePhotoService.Infrastructure.Domain;

public class PhotoRepository(IAmazonDynamoDB _client, IOptions<DynamoDBOptions> _options)
    : DynamoDbRepository<Photo, Guid>(_client, _options), IPhotoRepository
{
    protected override Document ToDocument(Photo entity)
    {
        var document = base.ToDocument(entity);
        document["PK"] = $"A#{entity.AlbumId}";
        document["SK"] = $"{document["SK"].AsString()}";
        return document;
    }

    public async Task<IList<Photo>> ListPhotosInAlbum(Guid albumId, CancellationToken cancellationToken = default)
    {
        var filter = new QueryFilter("SK", QueryOperator.BeginsWith, "P#");
        var results = await Table.Query($"A#{albumId}", filter).GetRemainingAsync(cancellationToken);
        return FromDocuments(results).ToList();
    }
}