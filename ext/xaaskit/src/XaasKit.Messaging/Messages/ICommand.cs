namespace XaasKit.Messaging;

public interface ICommand<out TResponse> : IMessage { }