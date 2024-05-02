using XaasKit.Amazon.DynamoDB;

namespace XaasKit.Amazon.DynamoDB.Repositories;

public class DynamoDBOptions
{
    public char? KeyPrefix { get; set; }
    
    public char KeySeparator { get; set; } = DynamoDBDefaults.KeySeparator;

    public string TableNamePrefix { get; set; } = string.Empty;
}