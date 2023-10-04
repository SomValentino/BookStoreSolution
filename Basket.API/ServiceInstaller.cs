using Microsoft.AspNetCore.Diagnostics;
using Newtonsoft.Json;

namespace Basket.API;

public static class ServiceInstaller {
    public static void UseException (this WebApplication app) {
        var logger = app.Services.GetService<ILogger<WebApplication>> ();
        app.UseExceptionHandler (option => {
            option.Run (async context => {
                context.Response.ContentType = "application/json";
                var exception = context.Features.Get<IExceptionHandlerPathFeature> ();
                logger?.LogError (exception?.Error.ToString ());
                await context.Response.WriteAsync (JsonConvert.SerializeObject (new {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                        ErrorMessage = "An Error occurred while processing your request. Kindly try again later"
                }));
            });
        });
    }

}