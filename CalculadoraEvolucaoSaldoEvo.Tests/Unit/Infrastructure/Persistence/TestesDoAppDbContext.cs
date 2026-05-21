using AwesomeAssertions;
using CalculadoraEvolucaoSaldoEvo.Application.UnitTests;
using CalculadoraEvolucaoSaldoEvo.Domain.Entities;
using CalculadoraEvolucaoSaldoEvo.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CalculadoraEvolucaoSaldoEvo.Infrastructure.UnitTests.Persistence;

public sealed class TestesDoAppDbContext
{
    [Fact]
    public async Task Evolucoes_DeSimulacaoSoftDeletada_NaoDevemSerRetornadasPeloFiltroGlobal()
    {
        await using var context = TestDbContextFactory.Create();

        var simulacaoAtiva = new Simulacao
        {
            ValorInicial = 1000m,
            TaxaJurosMensal = 1.5m,
            PrazoMeses = 1,
            ValorTotalFinal = 1015m,
            TotalJuros = 15m,
            Deletado = false,
            Evolucoes =
            [
                new Evolucao
                {
                    Mes = 1,
                    SaldoInicial = 1000m,
                    Juro = 15m,
                    SaldoFinal = 1015m
                }
            ]
        };

        var simulacaoSoftDeletada = new Simulacao
        {
            ValorInicial = 2000m,
            TaxaJurosMensal = 2m,
            PrazoMeses = 1,
            ValorTotalFinal = 2040m,
            TotalJuros = 40m,
            Deletado = true,
            Evolucoes =
            [
                new Evolucao
                {
                    Mes = 1,
                    SaldoInicial = 2000m,
                    Juro = 40m,
                    SaldoFinal = 2040m
                }
            ]
        };

        context.Simulacoes.AddRange(simulacaoAtiva, simulacaoSoftDeletada);
        await context.SaveChangesAsync();

        var evolucoesVisiveis = await context.Evolucoes
            .OrderBy(evolucao => evolucao.SimulacaoId)
            .ToListAsync();

        var todasAsEvolucoes = await context.Evolucoes
            .IgnoreQueryFilters()
            .ToListAsync();

        evolucoesVisiveis.Should().HaveCount(1);
        evolucoesVisiveis[0].SaldoFinal.Should().Be(1015m);
        todasAsEvolucoes.Should().HaveCount(2);
    }
}