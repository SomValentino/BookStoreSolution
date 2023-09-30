using BookCatalog.API.Middlewares;

namespace BookCatalog.API.Extensions;

public static class ApplicationBuilderExtensions {
    public static IApplicationBuilder UseCorrelationIdMiddleware (this IApplicationBuilder applicationBuilder) 
                        => applicationBuilder.UseMiddleware<CorrelationIdMiddleware> ();
}