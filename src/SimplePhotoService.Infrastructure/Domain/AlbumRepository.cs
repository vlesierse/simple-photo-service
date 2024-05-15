using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Microsoft.Extensions.Options;
using SimplePhotoService.Domain.Entities;
using SimplePhotoService.Domain.Repositories;
using XaasKit.Amazon.DynamoDB.Repositories;

namespace SimplePhotoService.Infrastructure.Domain;

public class AlbumRepository(IAmazonDynamoDB _client, IOptions<DynamoDBOptions> _options)
    : DynamoDbRepository<Album, Guid>(_client, _options), IAlbumRepository
{
    protected override Document ToDocument(Album entity)
    {
        var document = base.ToDocument(entity);
        document["OwnerSK"] = entity.LastUpdatedAt.ToUnixTimeSeconds().ToString();
        return document;
    }

    public async Task<IList<Album>> ListAblumsForOwner(string ownerId, CancellationToken cancellationToken = default)
    {
        var filter = new QueryFilter("OwnerId", QueryOperator.Equal, ownerId);
        var results = await Table.Query(new QueryOperationConfig { IndexName = "OwnerIndex", Filter = filter}).GetRemainingAsync(cancellationToken);
        return FromDocuments(results).ToList();
    }
}