using Amazon.DynamoDBv2.Model;

namespace XaasKit.Amazon.DynamoDB.Client;

public class DynamoDBItem(string tableName, string partitionKey, string? sortKey, IDictionary<string, AttributeValue>? values)
{
    
    public string TableName { get; } = tableName;
    
    public string PartitionKey { get; } = partitionKey;
    
    public string? SortKey { get; } = sortKey;
    
    public IDictionary<string, AttributeValue> Values { get; } = values ?? new Dictionary<string, AttributeValue>();

}