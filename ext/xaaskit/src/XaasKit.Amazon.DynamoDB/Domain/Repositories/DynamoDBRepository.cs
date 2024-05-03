using System.Text.Json;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Microsoft.Extensions.Options;
using XaasKit.Core;
using XaasKit.Domain.Entities;
using XaasKit.Domain.Repositories;

namespace XaasKit.Amazon.DynamoDB.Repositories;

public class DynamoDbRepository<TEntity>(IAmazonDynamoDB _client, IOptions<DynamoDBOptions> _options) : Repository<TEntity>
    where TEntity : class, IEntity
{
    protected DynamoDBOptions Options { get; } = _options.Value;

    private Table? _table;
    protected Table Table => _table ??= Table.LoadTable(_client, GetTableName());
    
    public override async Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var document = ToDocument(entity);
        var result = await Table.PutItemAsync(document, cancellationToken);
        return FromDocument(result);
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
        var search = Table.Scan(new ScanFilter());
        return Task.FromResult((long)search.Count);
    }

    public override async Task<List<TEntity>> GetListAsync(CancellationToken cancellationToken = default)
    {
        var search = Table.Scan(new ScanFilter());
        var documents = await search.GetRemainingAsync(cancellationToken);
        return documents.Select(FromDocument).ToList();
    }

    public override Task<List<TEntity>> GetPagedListAsync(int skip, int maxResults, string sorting,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new List<TEntity>());
    }

    private readonly string DefaultKeyPrefix = typeof(TEntity).Name.ToUpper()[..1];
    protected virtual string GetKeyPrefix() => DefaultKeyPrefix;

    protected virtual string GetPartitionKey(TEntity entity)
        => ComposeItemKey(entity.GetKeys()!);
    
    protected virtual string GetSortKey(TEntity entity)
        => ComposeItemKey(entity.GetKeys()!);

    protected virtual string? GetIndex(TEntity entity)
        => null;

    protected virtual string GetTableName()
    {
        return Options.TableName ?? Options.TableNamePrefix + typeof(TEntity).Name + "s";
    }

    protected virtual Document ToDocument(TEntity entity)
    {
        var sortKey = GetSortKey(entity);
        var partitionKey = GetPartitionKey(entity);
        var document = Document.FromJson(JsonSerializer.Serialize(entity));
        document[DynamoDBDefaults.PartitionKeyAttribute] = partitionKey;
        document[DynamoDBDefaults.SortKeyAttribute] = sortKey;
        return document;
    }
    
    protected virtual TEntity FromDocument(Document document)
    {
        var result = JsonSerializer.Deserialize<TEntity>(document.ToJson());
        return result ?? throw new XaasKitException("Unable to deserialize document to entity");
    }

    protected string ComposeItemKey(params object[] keys)
        => $"{GetKeyPrefix()}{Options.KeySeparator}{keys.JoinAsString(Options.KeySeparator)}";
}

public class DynamoDbRepository<TEntity, TKey>(IAmazonDynamoDB _client, IOptions<DynamoDBOptions> _options) : DynamoDbRepository<TEntity>(_client, _options), IRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
{
    public async Task<TEntity> GetAsync(TKey id, CancellationToken cancellationToken = default)
    {
        return await FindAsync(id, cancellationToken) ?? throw new EntityNotFoundException(typeof(TEntity), id);
    }

    public async Task<TEntity?> FindAsync(TKey id, CancellationToken cancellationToken = default)
    {
        var composedKey = ComposeItemKey(id!);
        var keys = new Dictionary<string, DynamoDBEntry>
        {
            [DynamoDBDefaults.PartitionKeyAttribute] = composedKey,
            [DynamoDBDefaults.SortKeyAttribute] = composedKey
        };
        var document = await Table.GetItemAsync(keys, cancellationToken);
        return document == null ? null : FromDocument(document);
    }

    public async Task<bool> DeleteAsync(TKey id, CancellationToken cancellationToken = default)
    {
        var composedKey = ComposeItemKey(id!);
        var keys = new Dictionary<string, DynamoDBEntry>
        {
            [DynamoDBDefaults.PartitionKeyAttribute] = composedKey,
            [DynamoDBDefaults.SortKeyAttribute] = composedKey
        };
        await Table.DeleteItemAsync(keys, cancellationToken);
        return true;
    }
}