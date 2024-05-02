using System.Globalization;
using SimplePhotoService.Api.Routes;

namespace SimplePhotoService.Api.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication ConfigureApplication(this WebApplication app, IWebHostEnvironment environment)
    {
        
        #region Security
        
        if (environment.IsDevelopment())
        {
            app.UseCors();
        }
        
        #endregion

        #region OpenAPI

        /*var ti = CultureInfo.CurrentCulture.TextInfo;

        _ = app.UseSwagger();
        _ = app.UseSwaggerUI(c =>
            c.SwaggerEndpoint(
                "/swagger/v1/swagger.json",
                $"CleanMinimalApi - {ti.ToTitleCase(app.Environment.EnvironmentName)} - V1"));
        */
        
        #endregion Swagger

        #region Endpoints

        _ = app.MapAlbumEndpoints();
        
        #endregion Endpoints
        
        return app;
    }
    
}