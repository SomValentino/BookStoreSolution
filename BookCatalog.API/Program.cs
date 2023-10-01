using BookCatalog.API.Data;
using Microsoft.EntityFrameworkCore;
using BookStore.Helpers.Extensions;
using BookCatalog.API.Extensions;
using BookCatalog.API.Services.interfaces;
using BookCatalog.API.Services;
using BookCatalog.API.Data.Repository;

var builder = WebApplication.CreateBuilder (args);

// Add services to the container.

builder.Services.AddControllers ();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer ();
builder.Services.AddSwaggerGen ();
builder.Services.AddDbContext<BookDbContext> (options => {
    options.UseNpgsql (builder.Configuration.GetConnectionString ("PostgreSQLConnection"));
    options.EnableSensitiveDataLogging ();
});
builder.Services.AddCorrelationIdGeneratorService ();
builder.Services.AddScoped<IBookCatalogService,BookCatalogService>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
builder.Services.AddScoped<IAuthorService,AuthorService>();

var app = builder.Build ();
using var scope = app.Services.CreateScope ();
var context = scope.ServiceProvider.GetRequiredService<BookDbContext> ();
context.Database.Migrate ();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment ()) {
    app.UseSwagger ();
    app.UseSwaggerUI ();
}

app.UseCorrelationIdMiddleware();
app.UseHttpsRedirection ();

app.UseAuthorization ();

app.MapControllers ();

app.Run ();