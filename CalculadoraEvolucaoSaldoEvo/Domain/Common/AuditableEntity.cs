namespace CalculadoraEvolucaoSaldoEvo.Domain.Common;

public abstract class AuditableEntity : BaseEntity
{
    public DateTimeOffset CriadoEm { get; set; }
    public string? CriadoPor { get; set; }
    public DateTimeOffset? ModificadoEm { get; set; }
    public string? ModificadoPor { get; set; }
}

