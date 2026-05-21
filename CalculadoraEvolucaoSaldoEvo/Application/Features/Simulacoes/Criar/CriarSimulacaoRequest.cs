namespace CalculadoraEvolucaoSaldoEvo.Application.Features.Simulacoes.Criar;

public record CriarSimulacaoRequest(
    decimal ValorInicial,
    decimal TaxaJurosMensal,
    int PrazoMeses
);

