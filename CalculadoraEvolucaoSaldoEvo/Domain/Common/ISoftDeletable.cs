namespace CalculadoraEvolucaoSaldoEvo.Domain.Common;

public interface ISoftDeletable
{
    bool Deletado { get; set; }
    DateTimeOffset? DeletadoEmUtc { get; set; }
}

