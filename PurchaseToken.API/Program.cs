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

builder.Services.AddAuthentication ("Bearer").AddJwtBearer ("Bearer", options => {
    options.Authority = builder.Configuration.GetValue<string> ("idp_url");
    options.RequireHttpsMetadata = false;

    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters {
        ValidateAudience = false
    };
});

builder.Services.AddAuthorization (options => {
    options.AddPolicy ("ScopePolicy", policy => {
        policy.RequireClaim ("scope", builder.Configuration.GetValue<string> ("ScopeName") !);
    });
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer ();
builder.Services.AddSwaggerGen ();

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