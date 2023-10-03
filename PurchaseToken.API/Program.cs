using System.Net;
using System.Text.Json;
using BookStore.Helpers.Extensions;
using IdentityModel.AspNetCore.OAuth2Introspection;
using IdentityServer4.AccessTokenValidation;
using Microsoft.IdentityModel.Logging;
using Microsoft.OpenApi.Models;
using PurchaseToken.API.Data;
using PurchaseToken.API.Data.Interfaces;
using PurchaseToken.API.Data.Repository;
using PurchaseToken.API.Data.Repository.Interfaces;
using PurchaseToken.API.Extensions;
using PurchaseToken.API.GrpcService;

var builder = WebApplication.CreateBuilder (args);
IdentityModelEventSource.ShowPII = true;

// Add services to the container.
builder.Services.AddScoped<ITokenAccountContext, TokenAccountContext> ();
builder.Services.AddScoped<ITokenAccountRepository, TokenAccountRepository> ();
builder.Services.AddControllers ();
builder.Services.AddCorrelationIdGeneratorService ();
builder.Services.AddGrpc ();

builder.Services.AddAuthentication (IdentityServerAuthenticationDefaults.AuthenticationScheme)
    .AddOAuth2Introspection (options => {
        options.Authority = builder.Configuration.GetValue<string> ("idp_url");
        options.ClientId = "PurchaseTokenAPI";
        options.ClientSecret = builder.Configuration.GetValue<string> ("client_secret");
    });

builder.Services.AddHttpClient (OAuth2IntrospectionDefaults.BackChannelHttpClientName)
    .ConfigurePrimaryHttpMessageHandler (() => new HttpClientHandler {
        ServerCertificateCustomValidationCallback = (_, _, _, _) => true
    });

builder.Services.AddAuthorization (options => {
    options.AddPolicy ("ScopePolicy", policy => {
        policy.RequireClaim ("scope", builder.Configuration.GetValue<string> ("ScopeName") !);
    });
});
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

var app = builder.Build ();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment ()) {
    app.UseSwagger ();
    app.UseSwaggerUI (c => {
        c.OAuthClientId (builder.Configuration.GetValue<string> ("client_id"));
    });
}

app.UseCorrelationIdMiddleware ();
app.UseAuthentication ();

app.UseAuthorization ();

app.MapControllers ();
app.MapGrpcService<PaymentGrpcService>();

app.Run ();