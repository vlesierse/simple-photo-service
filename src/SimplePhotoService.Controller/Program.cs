using Amazon;
using Amazon.BedrockRuntime;
using Amazon.Rekognition;
using Amazon.Runtime;
using Amazon.SQS;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SimplePhotoService.Controller;
using SimplePhotoService.Controller.Handlers;
using SimplePhotoService.Controller.Processing;
using SimplePhotoService.Infrastructure;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddTransient<IImageProcessor, CleanProcessor>();
builder.Services.AddTransient<IImageProcessor, LabelDetectionProcessor>();
builder.Services.AddTransient<IImageProcessor, ModerationProcessor>();
builder.Services.AddTransient<IImageProcessor, SmartCropProcessor>();
builder.Services.AddOptions().Configure<SmartCropOptions>(builder.Configuration.GetSection("Processors:SmartCrop"));

builder.Services
    .AddAWSService<IAmazonSQS>()
    .AddAWSService<IAmazonRekognition>()
    .AddSingleton<IAmazonBedrockRuntime>(new AmazonBedrockRuntimeClient(FallbackCredentialsFactory.GetCredentials(new AmazonBedrockRuntimeConfig()), RegionEndpoint.USWest2))
    .AddOptions().Configure<SQSQueueOptions>(builder.Configuration.GetSection("AWS:Resources:Queue"));
builder.Services
    .AddHostedService<SQSQueueProcessor>()
    .AddTransient<IMessageHandler, S3EventMessageHandler>();


var host = builder.Build();
host.Run();