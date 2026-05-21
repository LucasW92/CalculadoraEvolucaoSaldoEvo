using CalculadoraEvolucaoSaldoEvo.Application.Abstractions.Persistence;
using CalculadoraEvolucaoSaldoEvo.Application.Common;
using CalculadoraEvolucaoSaldoEvo.Domain.Common;

namespace CalculadoraEvolucaoSaldoEvo.Application.Features.Simulacoes.Consultar;

public sealed class ConsultarSimulacaoHandler(ISimulacaoRepositorio simulacaoRepositorio)
{
    public async Task<Result<ConsultarSimulacaoResponse>> Handle(int id, CancellationToken cancellationToken)
    {
        var simulacao = await simulacaoRepositorio.ObterPorIdComEvolucoesAsync(id, cancellationToken);

        if (simulacao is null)
        {
            return Result.Failure<ConsultarSimulacaoResponse>(
                Error.NotFound("Simulacao.NaoEncontrada", string.Format(Mensagens.SimulacaoNaoEncontrada, id))
            );
        }

        return Result.Success(simulacao.ToResponseDto());
    }
}

