using Amazon.DynamoDBv2.Model;
using XaasKit.Amazon.DynamoDB.Client;
using XaasKit.Domain.Entities;
using XaasKit.Domain.Repositories;

namespace XaasKit.Amazon.DynamoDB.Repositories;

public class DynamoDbRepository<TEntity>(IDynamoDBClient _client) : Repository<TEntity>
    where TEntity : class, IEntity
{
    protected DynamoDBOptions Options { get; } = _client.Options;
    
    public override async Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var item = ToDynamoDBItem(entity);
        await _client.PutItemAsync(item, cancellationToken);
        return entity;
    }

    public override Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override Task<bool> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override Task<long> GetCountAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override Task<List<TEntity>> GetListAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new List<TEntity>());
    }

    public override Task<List<TEntity>> GetPagedListAsync(int skip, int maxResults, string sorting,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new List<TEntity>());
    }

    protected virtual string GetPartitionKey(TEntity entity)
        => ComposeItemKey(entity.GetKeys()!);
    
    protected virtual string GetSortKey(TEntity entity)
        => ComposeItemKey(entity.GetKeys()!);

    protected virtual string? GetIndex(TEntity entity)
        => null;

    protected virtual string? GetTableName(TEntity entity)
    {
        return entity.GetType().Name + "s";
    }

    protected virtual DynamoDBItem ToDynamoDBItem(TEntity entity)
    {
        var index = GetIndex(entity);
        var sortKey = GetSortKey(entity);
        var partitionKey = GetPartitionKey(entity);
        return new DynamoDBItem(partitionKey, sortKey, index, new Dictionary<string, AttributeValue>());
    }

    private string ComposeItemKey(params object[] keys)
    {
        var keyPrefix = Options.KeyPrefix ?? GetType().Name.ToUpper()[0];
        return $"{Options.KeyPrefix}{Options.KeyPrefix.HasValue}{keys.JoinAsString(Options.KeySeparator)}";
    }
}