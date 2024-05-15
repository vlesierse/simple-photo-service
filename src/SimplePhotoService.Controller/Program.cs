using Amazon.SQS;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SimplePhotoService.Controller;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.Services
    .AddAWSService<IAmazonSQS>()
    .AddOptions().Configure<SQSQueueOptions>(builder.Configuration.GetSection("AWS:Resources:Queue"));
builder.Services.AddHostedService<SQSQueueProcessor>();


var host = builder.Build();
host.Run();