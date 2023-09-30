using BookStore.Helpers.Features;
using BookStore.Helpers.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace BookStore.Helpers.Extensions;

public static class ServiceCollectionExtensions {
    public static IServiceCollection AddCorrelationIdGeneratorService (this IServiceCollection services) {
        services.AddScoped<ICorrelationGenerator, CorrelationIdGenerator> ();

        return services;
    }
}