using System.Buffers.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Amazon.BedrockRuntime;
using Amazon.BedrockRuntime.Model;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;

namespace SimplePhotoService.Controller.Processing;

public class CleanProcessor(IAmazonBedrockRuntime bedrock, IOptions<CleanOptions> options) : IImageProcessor
{
    public CleanOptions Options { get; } = options.Value;

    public int Priority { get; } = 0;

    public async Task ProcessImageAsync(ImageProcessorContext context, CancellationToken cancellationToken)
    {
        if (context.Metadata.ExplicitContent)
        {
            return;
        }
        using var stream = new MemoryStream();
        await context.Image.SaveAsJpegAsync(stream, cancellationToken);
        
        var imageData = await RemoveBackground(stream.ToArray(), context, cancellationToken);
        imageData = await ImproveImageQuality(imageData!, context, cancellationToken);
        context.Image = Image.Load(imageData!);
    }

    #region Remove Background

    private async Task<byte[]?> RemoveBackground(byte[] imageData, ImageProcessorContext context, CancellationToken cancellationToken)
    {
        var imageBase64 = Convert.ToBase64String(imageData);
        var body = new BackgroundRemovalRequest("plain white background", imageBase64)
        {
            OutPaintingParams =
            {
                MaskPrompt = context.Metadata.Labels.OrderByDescending(l => l.Confidence).FirstOrDefault()?.Name ?? "Food"
            }
        };
        var response = await bedrock.InvokeModelAsync(new InvokeModelRequest
        {
            ModelId = "amazon.titan-image-generator-v2:0",
            Body = new MemoryStream(JsonSerializer.SerializeToUtf8Bytes(body))
        }, cancellationToken);
        
        var responseBody = JsonSerializer.Deserialize<TitanImageResponse>(response.Body);
        if (responseBody == null || responseBody.Images.Length == 0)
        {
            return null;
        }

        return Convert.FromBase64String(responseBody.Images[0]);
    }
    
    private abstract class TitanImageRequest(string taskType)
    {
        [JsonPropertyName("taskType")]
        public string TaskType { get; } = taskType;
    }

    private class TitanImageResponse()
    {
        [JsonPropertyName("images")]
        public required string[] Images { get; set; }
    }
    
    private class BackgroundRemovalRequest(string text, string imageBase64) : TitanImageRequest("OUTPAINTING")
    {
        [JsonPropertyName("outPaintingParams")]
        public OutPaintingParams OutPaintingParams { get; } =
            new OutPaintingParams { Image = imageBase64, Text = text};
        
        [JsonPropertyName("imageGenerationConfig")]
        public ImageGenerationConfig ImageGenerationConfig { get; set; } = new ImageGenerationConfig();
    }
    
    private class OutPaintingParams
    {
        [JsonPropertyName("text")]
        public required string Text { get; set; }
        
        [JsonPropertyName("image")]
        public required string Image { get; set; }
        
        [JsonPropertyName("maskPrompt")]
        public string? MaskPrompt { get; set; }

        [JsonPropertyName("outPaintingMode")]
        public string OutPaintingMode { get; set; } = "PRECISE";
    }

    private class ImageGenerationConfig
    {
        [JsonPropertyName("cfgScale")]
        public float CfgScale { get; set; } = 5.0f;

        [JsonPropertyName("numberOfImages")]
        public int NumberOfImages { get; set; } = 1;
        
        [JsonPropertyName("width")]
        public int Width { get; set; } = 1024;
        
        [JsonPropertyName("height")]
        public int Heigth { get; set; } = 1024;
    }

    #endregion
    
    #region Improve Quality
    
    private async Task<byte[]?> ImproveImageQuality(byte[] imageData, ImageProcessorContext context, CancellationToken cancellationToken)
    {
        var imageBase64 = Convert.ToBase64String(imageData);
        var topic = context.Metadata.Labels.OrderByDescending(l => l.Confidence).FirstOrDefault()?.Name ?? "Food";
        var body = new StableDiffusionRequest
        {
            Prompt = $"{topic} on a plain white background, centered in frame. High-quality professional food photography, sharp focus, vibrant colors, soft natural lighting. Only include ingredients that are in the reference image. Background blurred, suggesting an upscale restaurant setting. Style reminiscent of an elegant restaurant menu, 4K resolution, high contrast, mouth-watering presentation.",
            Image = imageBase64
        };
        var response = await bedrock.InvokeModelAsync(new InvokeModelRequest
        {
            ModelId = "stability.sd3-large-v1:0",
            Body = new MemoryStream(JsonSerializer.SerializeToUtf8Bytes(body))
        }, cancellationToken);
        
        var responseBody = JsonSerializer.Deserialize<StableDiffusionResponse>(response.Body);
        if (responseBody == null || responseBody.Images.Length == 0)
        {
            return null;
        }

        return Convert.FromBase64String(responseBody.Images[0]);
    }

    private class StableDiffusionRequest
    {
        [JsonPropertyName("prompt")]
        public required string Prompt { get; set; }
        
        [JsonPropertyName("image")]
        public required string Image { get; set; }

        [JsonPropertyName("strength")]
        public float Strength { get; set; } = 0.5f;
        
        [JsonPropertyName("output_format")]
        public string OutputFormat { get; set; } = "jpeg";

        [JsonPropertyName("mode")]
        public string Mode { get; set; } = "image-to-image";
    }

    private class StableDiffusionResponse()
    {
        [JsonPropertyName("images")]
        public required string[] Images { get; set; }
    }
    
    #endregion
}