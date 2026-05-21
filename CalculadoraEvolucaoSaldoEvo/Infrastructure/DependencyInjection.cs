using CalculadoraEvolucaoSaldoEvo.Application.Abstractions.Data;
using CalculadoraEvolucaoSaldoEvo.Application.Abstractions.Persistence;
using CalculadoraEvolucaoSaldoEvo.Infrastructure.Persistence;
using CalculadoraEvolucaoSaldoEvo.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CalculadoraEvolucaoSaldoEvo.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPersistence(configuration);
        services.AddCachingServices();

        return services;
    }

    private static void AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<SoftDeleteInterceptor>();

        services.AddDbContext<AppDbContext>((sp, options) =>
        {
            var interceptor = sp.GetRequiredService<SoftDeleteInterceptor>();
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection"))
                   .AddInterceptors(interceptor);
        });

        services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());

        services.AddScoped<ISimulacaoRepositorio, SimulacaoRepositorio>();
    }

    private static void AddCachingServices(this IServiceCollection services)
    {
        services.AddHybridCache(options =>
        {
            options.DefaultEntryOptions = new Microsoft.Extensions.Caching.Hybrid.HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromMinutes(5),
                LocalCacheExpiration = TimeSpan.FromMinutes(2)
            };
        });
    }
}

