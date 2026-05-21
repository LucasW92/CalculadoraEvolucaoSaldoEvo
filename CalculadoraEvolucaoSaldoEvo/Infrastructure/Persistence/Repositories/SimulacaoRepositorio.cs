using CalculadoraEvolucaoSaldoEvo.Application.Abstractions.Persistence;
using CalculadoraEvolucaoSaldoEvo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CalculadoraEvolucaoSaldoEvo.Infrastructure.Persistence.Repositories;

public sealed class SimulacaoRepositorio(AppDbContext context) : ISimulacaoRepositorio
{
    public async Task<Simulacao?> ObterPorIdComEvolucoesAsync(int id, CancellationToken cancellationToken)
    {
        return await context.Set<Simulacao>()
            .Include(s => s.Evolucoes.OrderBy(e => e.Mes))
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public void Adicionar(Simulacao simulacao)
    {
        context.Set<Simulacao>().Add(simulacao);
    }

    public void Remover(Simulacao simulacao)
    {
        context.Set<Simulacao>().Remove(simulacao);
    }
}

