using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Microsoft.Extensions.Options;
using XaasKit.Amazon.DynamoDB.Repositories;

namespace XaasKit.Amazon.DynamoDB.Client;

public class DynamoDBClient(IAmazonDynamoDB dynamodb, IOptions<DynamoDBOptions> options) : IDynamoDBClient
{
    public DynamoDBOptions Options { get; } = options.Value;
    Task<DynamoDBItem> IDynamoDBClient.PutItemAsync(DynamoDBItem item, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<DynamoDBItem> GetItemAsync(string tableName, string partitionKey, string? sortKey,
        CancellationToken cancellationToken = default)
    {
        var table = Table.LoadTable(dynamodb, tableName);
        var item = await table.GetItemAsync(partitionKey);
        throw new NotImplementedException();
    }
}