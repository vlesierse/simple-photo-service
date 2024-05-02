using XaasKit.Core;

namespace XaasKit.Messaging;

public class InvalidMessageException(object? message) : XaasKitException(
    "Tried to send/publish invalid message type to Mediator: " + message?.GetType().FullName ?? "Unknown")
{
    public object? MediatorMessage { get; } = message;
}