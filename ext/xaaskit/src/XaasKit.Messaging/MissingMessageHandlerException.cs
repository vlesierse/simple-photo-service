namespace XaasKit.Messaging;

/// <summary>
/// Exception that is thrown when XaasKit receives messages that have no registered handlers.
/// </summary>
public class MissingMessageHandlerException(object? message)
    : Exception("No handler registered for message type: " + message?.GetType().FullName ?? "Unknown")
{
    public object? MediatorMessage { get; } = message;
}