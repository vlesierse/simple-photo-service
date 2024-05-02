using SimplePhotoService.Application;
using SimplePhotoService.Infrastructure;

namespace SimplePhotoService.Api.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder ConfigureApplicationBuilder(this WebApplicationBuilder builder)
    {
        #region Serialisation

       /*_ = builder.Services.Configure<JsonOptions>(opt =>
        {
            opt.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            opt.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            opt.SerializerOptions.PropertyNameCaseInsensitive = true;
            opt.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            opt.SerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        });
        */
       
        #endregion Serialisation
        
        #region Swagger

        /*var ti = CultureInfo.CurrentCulture.TextInfo;

        _ = builder.Services.AddEndpointsApiExplorer();
        _ = builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1",
                new OpenApiInfo
                {
                    Version = "v1",
                    Title = $"CleanMinimalApi API - {ti.ToTitleCase(builder.Environment.EnvironmentName)}",
                    Description = "An example to share an implementation of Minimal API in .NET 6.",
                    Contact = new OpenApiContact
                    {
                        Name = "CleanMinimalApi API",
                        Email = "cleanminimalapi@stphnwlsh.dev",
                        Url = new Uri("https://github.com/stphnwlsh/cleanminimalapi")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "CleanMinimalApi API - License - MIT",
                        Url = new Uri("https://opensource.org/licenses/MIT")
                    },
                    TermsOfService = new Uri("https://github.com/stphnwlsh/cleanminimalapi")
                });

            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";

            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            options.DocInclusionPredicate((name, api) => true);
        });*/

        #endregion Swagger
        
        # region CORS
        
        if (builder.Environment.IsDevelopment())
        {
            _ = builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
                });
            });
        }
        
        #endregion

        _ = builder.Services.AddApplicationServices();
        _ = builder.Services.AddInfrastructureServices(builder.Configuration);

        #region AWS Lambda

        _ = builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);

        #endregion
        
        return builder;
    }
}