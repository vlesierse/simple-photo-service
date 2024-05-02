using RuntimeEnvironment = System.Environment;

namespace Constructs;

public static class ConstructExtensions
{
    public static bool IsTest(this IConstruct construct) => construct.IsEnvironment("test");

    public static bool IsProduction(this IConstruct construct) => construct.IsEnvironment("prod", "production");

    public static bool IsDevelopment(this IConstruct construct) => construct.IsEnvironment("dev", "development");

    public static string GetContext(this IConstruct construct, string key, string defaultValue)
    {
        return construct.TryGetContext(key, out var value) ? value! : defaultValue;
    }

    public static bool TryGetContext(this IConstruct construct, string key, out string? value)
    {
        var context = (string)construct.Node.TryGetContext(key);
        if (!string.IsNullOrEmpty(context))
        {
            value = context;
            return true;
        }
        value = null;
        return false;
    }

    #region Environment

    public static bool IsEnvironment(this IConstruct construct, params string[] environments)
    {
        return environments.Contains(construct.GetEnvironmentName());
    }

    public static void WhenEnvironment(this IConstruct construct, string environmentName, Action action)
    {
        if (construct.IsEnvironment(environmentName))
        {
            action.Invoke();
        }
    }

    public static string GetEnvironmentName(this IConstruct construct)
    {
        return construct.GetContext("env", RuntimeEnvironment.GetEnvironmentVariable("ENVIRONMENT") ?? "dev");
    }

    #endregion

    #region Tagging

    public static T AddTag<T>(this T construct, string key, string value)
        where T : Construct
    {
        Tags.Of(construct).Add(key, value);
        return construct;
    }

    public static T AddTags<T>(this T construct, IDictionary<string, string> tags)
        where T : Construct
    {
        foreach (var tag in tags)
        {
            construct.AddTag(tag.Key, tag.Value);
        }
        return construct;
    }

    #endregion
}
