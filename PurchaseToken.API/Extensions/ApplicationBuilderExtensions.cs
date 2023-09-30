using PurchaseToken.API.Middlewares;

namespace PurchaseToken.API.Extensions;

public static class ApplicationBuilderExtensions {
    public static IApplicationBuilder UseCorrelationIdMiddleware (this IApplicationBuilder applicationBuilder) 
                        => applicationBuilder.UseMiddleware<CorrelationIdMiddleware> ();
}