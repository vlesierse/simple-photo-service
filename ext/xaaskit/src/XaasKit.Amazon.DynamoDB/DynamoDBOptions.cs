namespace XaasKit.Amazon.DynamoDB.Repositories;

public class DynamoDBOptions
{
    public char KeySeparator { get; set; } = DynamoDBDefaults.KeySeparator;

    public string? TableName { get; set; }
    
    public string? TableNamePrefix { get; set; }
}