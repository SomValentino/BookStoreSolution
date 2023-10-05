using BookStore.Helpers.Features;
using BookStore.Helpers.Interfaces;
using EventBus.Messages.Common;
using Microsoft.Extensions.Primitives;

namespace PurchaseToken.API.Middlewares;

public class CorrelationIdMiddleware {
    private readonly RequestDelegate _next;
    private readonly ILogger<CorrelationIdMiddleware> _logger;

    public CorrelationIdMiddleware (RequestDelegate next, ILogger<CorrelationIdMiddleware> logger) {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke (HttpContext context, ICorrelationGenerator correlationIdGenerator) {
        var correlationId = GetCorrelationId (context, correlationIdGenerator);
        AddCorrelationIdHeaderToResponse (context, correlationId);
        if(!string.IsNullOrEmpty(correlationIdGenerator.Get())){
            _logger.LogInformation("Handling request with correlationId {id}", correlationId!);
        }
        await _next (context);
    }

    private static StringValues GetCorrelationId (HttpContext context, ICorrelationGenerator correlationIdGenerator) {
        if (context.Request.Headers.TryGetValue (EventBusConstants._correlationIdHeader, out var correlationId)) {
            correlationIdGenerator.Set (correlationId!);
            return correlationId;
        } else {
            return correlationIdGenerator.Get ();
        }
    }

    private static void AddCorrelationIdHeaderToResponse (HttpContext context, StringValues correlationId) {
        context.Response.OnStarting (() => {
            context.Response.Headers.Add (EventBusConstants._correlationIdHeader, new [] { correlationId.ToString () });
            return Task.CompletedTask;
        });
    }
}