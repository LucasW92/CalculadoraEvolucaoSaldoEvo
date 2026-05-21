using CalculadoraEvolucaoSaldoEvo.Application.Features.Simulacoes.Criar;
using CalculadoraEvolucaoSaldoEvo.Application.Features.Simulacoes.Consultar;
using FluentValidation;

namespace CalculadoraEvolucaoSaldoEvo.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddValidatorsFromAssembly(assembly);

        services.AddScoped<CriarSimulacaoHandler>();
        services.AddScoped<ConsultarSimulacaoHandler>();

        return services;
    }
}

