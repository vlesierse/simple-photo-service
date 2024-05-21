using System.Text.Json.Serialization;
using Amazon.Lambda.S3Events;

namespace SimplePhotoService.Controller.Handlers;

public class S3EventMessage
{
    [JsonPropertyName("Records")] public IEnumerable<S3Event.S3EventNotificationRecord> Records { get; init; } = [];

    public static S3EventMessage Empty = new();
}