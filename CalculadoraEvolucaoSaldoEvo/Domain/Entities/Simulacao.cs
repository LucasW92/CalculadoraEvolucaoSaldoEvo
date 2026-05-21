using CalculadoraEvolucaoSaldoEvo.Domain.Common;

namespace CalculadoraEvolucaoSaldoEvo.Domain.Entities;

public sealed class Simulacao : AuditableEntity, ISoftDeletable
{
    public decimal ValorInicial { get; set; }
    public decimal TaxaJurosMensal { get; set; }
    public int PrazoMeses { get; set; }
    public decimal ValorTotalFinal { get; set; }
    public decimal TotalJuros { get; set; }

    public ICollection<Evolucao> Evolucoes { get; set; } = new List<Evolucao>();

    public bool Deletado { get; set; }
    public DateTimeOffset? DeletadoEmUtc { get; set; }
}
