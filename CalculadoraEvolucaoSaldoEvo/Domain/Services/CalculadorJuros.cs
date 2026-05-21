using CalculadoraEvolucaoSaldoEvo.Domain.Common;

namespace CalculadoraEvolucaoSaldoEvo.Domain.Services;

public record ItemEvolucao(int Mes, decimal SaldoInicial, decimal Juro, decimal SaldoFinal);

public record ResultadoCalculo(decimal ValorTotalFinal, decimal TotalJuros, IReadOnlyList<ItemEvolucao> Evolucoes);

public static class CalculadorJuros
{
    private const int PrazoMaximoMeses = 1200;

    public static Result<ResultadoCalculo> Calcular(decimal valorInicial, decimal taxaJurosMensal, int prazoMeses)
    {
        if (valorInicial <= 0)
        {
            return Result.Failure<ResultadoCalculo>(
                Error.Validation("Calculo.ValorInicialInvalido", "O valor inicial deve ser maior que zero."));
        }

        if (taxaJurosMensal < 0)
        {
            return Result.Failure<ResultadoCalculo>(
                Error.Validation("Calculo.TaxaJurosInvalida", "A taxa de juros mensal deve ser maior ou igual a zero."));
        }

        if (prazoMeses <= 0 || prazoMeses > PrazoMaximoMeses)
        {
            return Result.Failure<ResultadoCalculo>(
                Error.Validation("Calculo.PrazoMesesInvalido", "O prazo deve ser maior que zero e menor ou igual a 1200 meses."));
        }

        var evolucoes = new List<ItemEvolucao>();
        var saldoAtual = valorInicial;
        var totalJuros = 0.0m;

        try
        {
            for (int mes = 1; mes <= prazoMeses; mes++)
            {
                var saldoInicial = saldoAtual;
                var juro = Math.Round(saldoInicial * (taxaJurosMensal / 100), 2, MidpointRounding.AwayFromZero);
                var saldoFinal = saldoInicial + juro;

                evolucoes.Add(new ItemEvolucao(mes, saldoInicial, juro, saldoFinal));

                saldoAtual = saldoFinal;
                totalJuros += juro;
            }
        }
        catch (OverflowException)
        {
            return Result.Failure<ResultadoCalculo>(
                Error.Validation("Calculo.LimiteExcedido", "O resultado da simulação excedeu o limite máximo permitido. Por favor, reduza algum dos parâmetros de simulação."));
        }

        return Result.Success(new ResultadoCalculo(
            ValorTotalFinal: saldoAtual,
            TotalJuros: totalJuros,
            Evolucoes: evolucoes
        ));
    }
}
