using CalculadoraEvolucaoSaldoEvo.Domain.Entities;

namespace CalculadoraEvolucaoSaldoEvo.Application.Features.Simulacoes.Criar;

public record EvolucaoResponseDto(
    int Mes,
    decimal SaldoInicial,
    decimal Juro,
    decimal SaldoFinal
);

public record CriarSimulacaoResponse(
    int Id,
    decimal ValorTotalFinal,
    decimal TotalJuros,
    IReadOnlyList<EvolucaoResponseDto> MemoriaCalculo
);

public static class MapeamentoExtensions
{
    public static CriarSimulacaoResponse ToResponseDto(this Simulacao simulacao)
    {
        return new CriarSimulacaoResponse(
            Id: simulacao.Id,
            ValorTotalFinal: simulacao.ValorTotalFinal,
            TotalJuros: simulacao.TotalJuros,
            MemoriaCalculo: simulacao.Evolucoes
                .OrderBy(e => e.Mes)
                .Select(e => new EvolucaoResponseDto(
                    Mes: e.Mes,
                    SaldoInicial: e.SaldoInicial,
                    Juro: e.Juro,
                    SaldoFinal: e.SaldoFinal
                ))
                .ToList()
        );
    }
}

