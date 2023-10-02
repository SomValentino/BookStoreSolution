using System.Text.Json;
using Basket.API.Repository;
using Basket.API.Services;
using BookCatalog.API.Extensions;
using BookStore.Helpers.Extensions;
using IdentityModel.AspNetCore.OAuth2Introspection;
using IdentityServer4.AccessTokenValidation;
using MassTransit;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder (args);

// Add services to the container.

builder.Services.AddControllers ();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer ();
builder.Services.AddSwaggerGen (options => {
    options.MapType<JsonDocument> (() => new OpenApiSchema { Type = "object" });

    options.AddSecurityDefinition ("Username and password login", new OpenApiSecurityScheme {
        Type = SecuritySchemeType.OAuth2,
            Flows = new OpenApiOAuthFlows () {
                Implicit = new OpenApiOAuthFlow {
                    AuthorizationUrl = new Uri (builder.Configuration.GetValue<string> ("authorize_url")),
                        TokenUrl = new Uri (builder.Configuration.GetValue<string> ("token_url")),
                        Scopes = builder.Configuration.GetValue<string> ("ScopeName").Split (' ').ToDictionary (_ => _)

                }
            }
    });

    options.AddSecurityRequirement (new OpenApiSecurityRequirement () {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                        Type = ReferenceType.SecurityScheme,
                            Id = "Username and password login"
                    },
                    Scheme = "oauth2",
                    Name = "Username and password login",
                    In = ParameterLocation.Header
            },
            new List<string> ()
        }
    });
});
builder.Services.AddAuthentication (IdentityServerAuthenticationDefaults.AuthenticationScheme)
    .AddOAuth2Introspection (options => {
        options.Authority = builder.Configuration.GetValue<string> ("idp_url");
        options.ClientId = "BasketAPI";
        options.SaveToken = true;
        options.ClientSecret = builder.Configuration.GetValue<string> ("client_secret");
    });

builder.Services.AddCorrelationIdGeneratorService ();

builder.Services.AddHttpClient (OAuth2IntrospectionDefaults.BackChannelHttpClientName)
    .ConfigurePrimaryHttpMessageHandler (() => new HttpClientHandler {
        ServerCertificateCustomValidationCallback = (_, _, _, _) => true
    });

builder.Services.AddStackExchangeRedisCache (options => {
    options.Configuration = builder.Configuration.GetValue<string> ("ConnectionStrings");
});
builder.Services.AddScoped<IBasketRepository, BasketRepository> ();
builder.Services.AddScoped<IBasketService, BasketService> ();
builder.Services.AddAutoMapper (typeof (Program));

builder.Services.AddMassTransit (config => {
    config.UsingRabbitMq ((ctx, cfg) => {
        cfg.Host (builder.Configuration["EventBusHostAddress"]);
    });
});
builder.Services.AddMassTransitHostedService ();

var app = builder.Build ();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment ()) {
    app.UseSwagger ();
    app.UseSwaggerUI (c => {
        c.OAuthClientId (builder.Configuration.GetValue<string> ("client_id"));
    });
}

app.UseCorrelationIdMiddleware ();
app.UseHttpsRedirection ();

app.UseAuthentication ();

app.UseAuthorization ();

app.MapControllers ();

app.Run ();