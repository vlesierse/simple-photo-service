namespace Amazon.CDK;

public static class EnvironmentExtensions
{
    public static Environment WithRegion(this Environment environment, string region) => new() { Account = environment.Account, Region = region };
}