using Order.API.Middlewares;

namespace Order.API.Extensions;

public static class ApplicationBuilderExtensions {
    public static IApplicationBuilder UseCorrelationIdMiddleware (this IApplicationBuilder applicationBuilder) 
                        => applicationBuilder.UseMiddleware<CorrelationIdMiddleware> ();
}