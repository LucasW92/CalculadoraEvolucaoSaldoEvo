using CalculadoraEvolucaoSaldoEvo.Domain.Entities;

namespace CalculadoraEvolucaoSaldoEvo.Application.Features.Simulacoes.Consultar;

public record EvolucaoConsultaDto(
    int Mes,
    decimal SaldoInicial,
    decimal Juro,
    decimal SaldoFinal
);

public record ConsultarSimulacaoResponse(
    int Id,
    decimal ValorTotalFinal,
    decimal TotalJuros,
    IReadOnlyList<EvolucaoConsultaDto> MemoriaCalculo
);

public static class MapeamentoExtensions
{
    public static ConsultarSimulacaoResponse ToResponseDto(this Simulacao simulacao)
    {
        return new ConsultarSimulacaoResponse(
            Id: simulacao.Id,
            ValorTotalFinal: simulacao.ValorTotalFinal,
            TotalJuros: simulacao.TotalJuros,
            MemoriaCalculo: simulacao.Evolucoes
                .OrderBy(e => e.Mes)
                .Select(e => new EvolucaoConsultaDto(
                    Mes: e.Mes,
                    SaldoInicial: e.SaldoInicial,
                    Juro: e.Juro,
                    SaldoFinal: e.SaldoFinal
                ))
                .ToList()
        );
    }
}

