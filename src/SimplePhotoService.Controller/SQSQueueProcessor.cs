using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using SimplePhotoService.Controller.Handlers;

namespace SimplePhotoService.Controller;

public class SQSQueueProcessor(IAmazonSQS client, IOptions<SQSQueueOptions> options, IEnumerable<IMessageHandler> handlers) : BackgroundService
{
    private SQSQueueOptions _options = options.Value;
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine($"Starting polling queue at {_options.QueueUrl}");
            
        while (!stoppingToken.IsCancellationRequested)
        {
            var messages = await ReceiveMessageAsync(client, _options.QueueUrl, 10);

            if (messages.Count != 0)
            {
                Console.WriteLine($"{messages.Count} messages received");
                foreach (var msg in messages)
                {
                    try
                    {
                        await ProcessMessage(msg, stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing message {msg.MessageId}: {ex.Message}");
                        continue;
                    }
                    Console.WriteLine($"{msg.MessageId} processed with success");
                    await DeleteMessageAsync(client, _options.QueueUrl, msg.ReceiptHandle);
                }
            }
            else
            {
                Console.WriteLine("No message available");
            }
        }
    }
    
    private Task ProcessMessage(Message message, CancellationToken cancellationToken = default)
    {
        return Task.WhenAll(handlers.Select(handler => handler.Handle(message, cancellationToken)));
    }
    
    private static async Task<List<Message>> ReceiveMessageAsync(IAmazonSQS client, string queueUrl, int maxMessages = 1)
    {
        var request = new ReceiveMessageRequest
        {
            QueueUrl = queueUrl,
            MaxNumberOfMessages = maxMessages,
            WaitTimeSeconds = 20,
            VisibilityTimeout = 30,
        };
        
        var messages = await client.ReceiveMessageAsync(request);
        return messages.Messages;
    }
    
    private static async Task DeleteMessageAsync(IAmazonSQS client,string queueUrl, string id)
    {
        var request = new DeleteMessageRequest
        {
            QueueUrl = queueUrl,
            ReceiptHandle = id
        };

        await client.DeleteMessageAsync(request);
    }
}