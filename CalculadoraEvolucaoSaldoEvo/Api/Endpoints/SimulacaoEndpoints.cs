using CalculadoraEvolucaoSaldoEvo.Api.Extensions;
using CalculadoraEvolucaoSaldoEvo.Application.Features.Simulacoes.Criar;
using CalculadoraEvolucaoSaldoEvo.Application.Features.Simulacoes.Consultar;

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

        return TypedResults.CreatedAtRoute(response, "ConsultarSimulacao", new { id = response.Id });
    }

    private static async Task<IResult> ConsultarSimulacao(
        int id,
        ConsultarSimulacaoHandler handler,
        ILoggerFactory loggerFactory,
        CancellationToken cancellationToken)
    {
        var logger = loggerFactory.CreateLogger("Api.Endpoints.Simulacoes");

        var result = await handler.Handle(id, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning(
                "Endpoint ConsultarSimulacao retornou falha. SimulacaoId: {SimulacaoId}; CodigoErro: {CodigoErro}; TipoErro: {TipoErro}",
                id,
                result.Error!.Code,
                result.Error.Type);

            return result.ToProblemDetails();
        }

        return TypedResults.Ok(result.Value);
    }
}
