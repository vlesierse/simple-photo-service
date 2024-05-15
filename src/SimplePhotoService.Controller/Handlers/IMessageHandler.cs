using Amazon.SQS.Model;

namespace SimplePhotoService.Controller.Handlers;

public interface IMessageHandler
{
    Task Handle(Message message, CancellationToken cancellationToken = default);
}