using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SimplePhotoService.Application;
using SimplePhotoService.Infrastructure;

namespace SimplePhotoService.Api.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder ConfigureApplicationBuilder(this WebApplicationBuilder builder)
    {
        # region CORS
        
        _ = builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
            });
        });
        
        #endregion
        
        #region Authentication

        _ = builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                var cognitoUserPoolId = builder.Configuration["AWS:Resources:Cognito:UserPoolId"];
                options.Authority = $"https://cognito-idp.{cognitoUserPoolId?.Split('_')[0]}.amazonaws.com/{cognitoUserPoolId}";
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidateAudience = false,
                };
            });
        _ = builder.Services.AddAuthorization();
        #endregion
        
        _ = builder.Services.AddApplicationServices();
        _ = builder.Services.AddInfrastructureServices(builder.Configuration);
        _ = builder.AddServiceDefaults();
        
        #region AWS Lambda

        _ = builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);

        #endregion
        
        return builder;
    }
}