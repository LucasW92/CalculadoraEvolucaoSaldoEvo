using CalculadoraEvolucaoSaldoEvo.Domain.Common;

namespace CalculadoraEvolucaoSaldoEvo.Domain.Entities;

public sealed class Evolucao : BaseEntity
{
    public int SimulacaoId { get; set; }
    public int Mes { get; set; }
    public decimal SaldoInicial { get; set; }
    public decimal Juro { get; set; }
    public decimal SaldoFinal { get; set; }

    public Simulacao Simulacao { get; set; } = null!;
}

