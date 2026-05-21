namespace CalculadoraEvolucaoSaldoEvo.Application.Abstractions.Data;

public interface IAppDbContext
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

