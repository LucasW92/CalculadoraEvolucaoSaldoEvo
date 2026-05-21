using CalculadoraEvolucaoSaldoEvo.Application.Abstractions.Data;
using CalculadoraEvolucaoSaldoEvo.Application.Abstractions.Persistence;
using CalculadoraEvolucaoSaldoEvo.Domain.Common;
using CalculadoraEvolucaoSaldoEvo.Domain.Entities;
using CalculadoraEvolucaoSaldoEvo.Domain.Services;

namespace CalculadoraEvolucaoSaldoEvo.Application.Features.Simulacoes.Criar;

public sealed class CriarSimulacaoHandler(
    ISimulacaoRepositorio simulacaoRepositorio,
    IAppDbContext unitOfWork,
    ILogger<CriarSimulacaoHandler> logger)
{
    public async Task<Result<CriarSimulacaoResponse>> Handle(
        CriarSimulacaoRequest request,
        CancellationToken cancellationToken)
    {
        var resultadoCalculo = CalculadorJuros.Calcular(
            request.ValorInicial,
            request.TaxaJurosMensal,
            request.PrazoMeses
        );

        if (resultadoCalculo.IsFailure)
        {
            logger.LogWarning(
                "Criação de simulação rejeitada. CodigoErro: {CodigoErro}; Mensagem: {MensagemErro}",
                resultadoCalculo.Error!.Code,
                resultadoCalculo.Error.Message);

            return Result.Failure<CriarSimulacaoResponse>(resultadoCalculo.Error);
        }

        var calculo = resultadoCalculo.Value!;
        var simulacao = new Simulacao
        {
            ValorInicial = request.ValorInicial,
            TaxaJurosMensal = request.TaxaJurosMensal,
            PrazoMeses = request.PrazoMeses,
            ValorTotalFinal = calculo.ValorTotalFinal,
            TotalJuros = calculo.TotalJuros,
            Evolucoes = calculo.Evolucoes.Select(e => new Evolucao
            {
                Mes = e.Mes,
                SaldoInicial = e.SaldoInicial,
                Juro = e.Juro,
                SaldoFinal = e.SaldoFinal
            }).ToList()
        };

        simulacaoRepositorio.Adicionar(simulacao);
        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Falha ao criar simulação. ValorInicial: {ValorInicial}; TaxaJurosMensal: {TaxaJurosMensal}; PrazoMeses: {PrazoMeses}",
                request.ValorInicial,
                request.TaxaJurosMensal,
                request.PrazoMeses);

            throw;
        }

        return Result.Success(simulacao.ToResponseDto());
    }
}
