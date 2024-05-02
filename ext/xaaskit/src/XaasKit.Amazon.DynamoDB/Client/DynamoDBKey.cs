namespace XaasKit.Amazon.DynamoDB.Client;

public struct DynamoDBKey(string PartitionKey, string? SortKey, string? Index) { }