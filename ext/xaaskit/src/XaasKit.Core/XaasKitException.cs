using System.Runtime.Serialization;

namespace XaasKit.Core;

/// <summary>
/// Base exception type for those are thrown as XaasKit specific exceptions.
/// </summary>
public class XaasKitException : Exception
{
    public XaasKitException() { }

    public XaasKitException(string? message)
        : base(message)
    { }

    public XaasKitException(string? message, Exception? innerException)
        : base(message, innerException)
    { }

    public XaasKitException(SerializationInfo serializationInfo, StreamingContext context)
        : base(serializationInfo, context)
    { }
}