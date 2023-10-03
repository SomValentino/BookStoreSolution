using System.Reflection;
using System.Text.Json;
using BookCatalog.API.Extensions;
using BookStore.Helpers.Extensions;
using EventBus.Messages.Common;
using FluentValidation;
using IdentityModel.AspNetCore.OAuth2Introspection;
using IdentityServer4.AccessTokenValidation;
using MassTransit;
using MediatR;
using Microsoft.OpenApi.Models;
using Order.API.Application.Behaviours;
using Order.API.Data;
using Order.API.Data.Interfaces;
using Order.API.Data.Repository;
using Order.API.Data.Repository.Interfaces;
using Order.API.Events;

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
        options.ClientId = "OrderAPI";
        options.SaveToken = true;
        options.ClientSecret = builder.Configuration.GetValue<string> ("client_secret");
    });

builder.Services.AddCorrelationIdGeneratorService ();

builder.Services.AddHttpClient (OAuth2IntrospectionDefaults.BackChannelHttpClientName)
    .ConfigurePrimaryHttpMessageHandler (() => new HttpClientHandler {
        ServerCertificateCustomValidationCallback = (_, _, _, _) => true
    });
builder.Services.AddScoped<IOrderContext, OrderContext> ();
builder.Services.AddScoped<IOrderRepository, OrderRepository> ();
builder.Services.AddScoped<BasketCheckoutConsumer> ();
builder.Services.AddValidatorsFromAssembly (Assembly.GetExecutingAssembly ());
builder.Services.AddMediatR (Assembly.GetExecutingAssembly ());

builder.Services.AddTransient (typeof (IPipelineBehavior<,>), typeof (UnhandledExceptionBehaviour<,>));
builder.Services.AddTransient (typeof (IPipelineBehavior<,>), typeof (ValidationBehaviour<,>));

builder.Services.AddMassTransit (config => {

    config.AddConsumer<BasketCheckoutConsumer> ();

    config.UsingRabbitMq ((ctx, cfg) => {
        cfg.Host (builder.Configuration["EventBusHostAddress"]);

        cfg.ReceiveEndpoint (EventBusConstants.BasketCheckoutQueue, c => {
            c.ConfigureConsumer<BasketCheckoutConsumer> (ctx);
        });
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
app.UseCorrelationIdMiddleware();
app.UseHttpsRedirection ();
app.UseAuthentication ();
app.UseAuthorization ();

app.MapControllers ();

app.Run ();