using CalculadoraEvolucaoSaldoEvo.Api.Extensions;
using CalculadoraEvolucaoSaldoEvo.Application.Features.Simulacoes.Criar;
using CalculadoraEvolucaoSaldoEvo.Application.Features.Simulacoes.Consultar;
using CalculadoraEvolucaoSaldoEvo.Application.Common;
using CalculadoraEvolucaoSaldoEvo.Domain.Common;
using Microsoft.Extensions.Caching.Hybrid;

namespace CalculadoraEvolucaoSaldoEvo.Api.Endpoints;

public static class SimulacaoEndpoints
{
    public static void MapSimulacaoEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/simulacoes")
            .WithTags("Simulações");

        group.MapPost("/", CriarSimulacao)
            .WithName("CriarSimulacao")
            .WithSummary("Realiza uma nova simulação de evolução de saldo com juros compostos")
            .WithDescription("Calcula a evolução do saldo mês a mês, persiste a simulação e retorna o identificador, o total de juros e a memória de cálculo.")
            .Accepts<CriarSimulacaoRequest>("application/json")
            .Produces<CriarSimulacaoResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .AddEndpointFilter<ValidationFilter<CriarSimulacaoRequest>>();

        group.MapGet("/{id:int}", ConsultarSimulacao)
            .WithName("ConsultarSimulacao")
            .WithSummary("Consulta uma simulação existente pelo seu ID")
            .WithDescription("Retorna os dados consolidados da simulação e a memória de cálculo detalhada para o identificador informado.")
            .Produces<ConsultarSimulacaoResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> CriarSimulacao(
        CriarSimulacaoRequest request,
        CriarSimulacaoHandler handler,
        HybridCache cache,
        ILoggerFactory loggerFactory,
        CancellationToken cancellationToken)
    {
        var logger = loggerFactory.CreateLogger("Api.Endpoints.Simulacoes");

        var result = await handler.Handle(request, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning(
                "Endpoint CriarSimulacao retornou falha. CodigoErro: {CodigoErro}; TipoErro: {TipoErro}",
                result.Error!.Code,
                result.Error.Type);

            return result.ToProblemDetails();
        }

        var response = result.Value!;

        // Pré-popula o cache já que a simulação acabou de ser criada
        var consultaResponse = new ConsultarSimulacaoResponse(
            Id: response.Id,
            ValorTotalFinal: response.ValorTotalFinal,
            TotalJuros: response.TotalJuros,
            MemoriaCalculo: response.MemoriaCalculo.Select(e => new EvolucaoConsultaDto(
                Mes: e.Mes,
                SaldoInicial: e.SaldoInicial,
                Juro: e.Juro,
                SaldoFinal: e.SaldoFinal
            )).ToList()
        );

        var cacheKey = $"simulacao:{response.Id}";
        await cache.SetAsync(cacheKey, consultaResponse, cancellationToken: cancellationToken);

        return TypedResults.CreatedAtRoute(response, "ConsultarSimulacao", new { id = response.Id });
    }

    private static async Task<IResult> ConsultarSimulacao(
        int id,
        ConsultarSimulacaoHandler handler,
        HybridCache cache,
        ILoggerFactory loggerFactory,
        CancellationToken cancellationToken)
    {
        var logger = loggerFactory.CreateLogger("Api.Endpoints.Simulacoes");
        var cacheKey = $"simulacao:{id}";

        var response = await cache.GetOrCreateAsync<ConsultarSimulacaoResponse?>(
            cacheKey,
            async token =>
            {
                var result = await handler.Handle(id, token);
                return result.IsSuccess ? result.Value : null;
            },
            cancellationToken: cancellationToken
        );

        if (response is null)
        {
            logger.LogWarning(
                "Endpoint ConsultarSimulacao retornou falha. SimulacaoId: {SimulacaoId}; CodigoErro: Simulacao.NaoEncontrada; TipoErro: NotFound",
                id);

            return Result.Failure<ConsultarSimulacaoResponse>(
                Error.NotFound("Simulacao.NaoEncontrada", string.Format(Mensagens.SimulacaoNaoEncontrada, id))
            ).ToProblemDetails();
        }

        return TypedResults.Ok(response);
    }
}
