using CalculadoraEvolucaoSaldoEvo.Domain.Entities;

namespace CalculadoraEvolucaoSaldoEvo.Application.Abstractions.Persistence;

public interface ISimulacaoRepositorio
{
    Task<Simulacao?> ObterPorIdComEvolucoesAsync(int id, CancellationToken cancellationToken);
    void Adicionar(Simulacao simulacao);
    void Remover(Simulacao simulacao);
}

