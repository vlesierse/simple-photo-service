using XaasKit.Amazon.DynamoDB.Repositories;

namespace XaasKit.Amazon.DynamoDB.Client;

public interface IDynamoDBClient
{
    
    DynamoDBOptions Options { get; }
    
    Task<DynamoDBItem> PutItemAsync(DynamoDBItem item, CancellationToken cancellationToken = default);
    
    Task<DynamoDBItem> GetItemAsync(string tableName, string partitionKey, string? sortKey, CancellationToken cancellationToken = default);
}