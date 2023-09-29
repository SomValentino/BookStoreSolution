using System.Net;
using IdentityModel.AspNetCore.OAuth2Introspection;
using IdentityServer4.AccessTokenValidation;
using Microsoft.IdentityModel.Logging;
using PurchaseToken.API.Data;
using PurchaseToken.API.Data.Interfaces;
using PurchaseToken.API.Data.Repository;
using PurchaseToken.API.Data.Repository.Interfaces;

var builder = WebApplication.CreateBuilder (args);
IdentityModelEventSource.ShowPII = true;

// Add services to the container.
builder.Services.AddScoped<ITokenAccountContext, TokenAccountContext> ();
builder.Services.AddScoped<ITokenAccountRepository, TokenAccountRepository> ();
builder.Services.AddControllers ();

builder.Services.AddAuthentication (IdentityServerAuthenticationDefaults.AuthenticationScheme)
    .AddOAuth2Introspection (options => {
        options.Authority = builder.Configuration.GetValue<string> ("idp_url");
        options.ClientId = "API";
        options.ClientSecret = builder.Configuration.GetValue<string> ("client_secret");
    });

builder.Services.AddHttpClient (OAuth2IntrospectionDefaults.BackChannelHttpClientName)
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler{
        ServerCertificateCustomValidationCallback = (_, _, _, _) => true
    });

        builder.Services.AddAuthorization (options => {
            options.AddPolicy ("ScopePolicy", policy => {
                policy.RequireClaim ("scope", builder.Configuration.GetValue<string> ("ScopeName") !);
            });
        });
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer (); builder.Services.AddSwaggerGen ();

        var app = builder.Build ();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment ()) {
            app.UseSwagger ();
            app.UseSwaggerUI ();
        }

        app.UseAuthentication ();

        app.UseAuthorization ();

        app.MapControllers ();

        app.Run ();