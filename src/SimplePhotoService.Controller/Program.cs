using Amazon.SQS;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SimplePhotoService.Controller;
using SimplePhotoService.Controller.Handlers;
using SimplePhotoService.Infrastructure;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services
    .AddAWSService<IAmazonSQS>()
    .AddOptions().Configure<SQSQueueOptions>(builder.Configuration.GetSection("AWS:Resources:Queue"));
builder.Services
    .AddHostedService<SQSQueueProcessor>()
    .AddTransient<IMessageHandler, S3EventMessageHandler>();


var host = builder.Build();
host.Run();