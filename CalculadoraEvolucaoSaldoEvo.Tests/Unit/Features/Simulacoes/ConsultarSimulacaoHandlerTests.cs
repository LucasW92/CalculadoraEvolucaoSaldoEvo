using AwesomeAssertions;
using CalculadoraEvolucaoSaldoEvo.Application.Abstractions.Persistence;
using CalculadoraEvolucaoSaldoEvo.Application.Features.Simulacoes.Consultar;
using CalculadoraEvolucaoSaldoEvo.Domain.Entities;
using NSubstitute;

namespace CalculadoraEvolucaoSaldoEvo.Application.UnitTests.Features.Simulacoes;

public class TestesDoHandlerConsultarSimulacao
{
    private readonly ISimulacaoRepositorio _simulacaoRepositorio = Substitute.For<ISimulacaoRepositorio>();
    private readonly ConsultarSimulacaoHandler _handler;

    public TestesDoHandlerConsultarSimulacao()
    {
        _handler = new ConsultarSimulacaoHandler(_simulacaoRepositorio);
    }

    [Fact]
    public async Task Handle_ComIdExistente_DeveRetornarSimulacao()
    {
        // Arrange
        int simulacaoId = 42;
        var cancellationToken = TestContext.Current.CancellationToken;
        var simulacao = new Simulacao
        {
            Id = simulacaoId,
            ValorInicial = 1000.00m,
            TaxaJurosMensal = 1.5m,
            PrazoMeses = 2,
            ValorTotalFinal = 1030.23m,
            TotalJuros = 30.23m,
            Evolucoes = new List<Evolucao>
            {
                new() { Mes = 1, SaldoInicial = 1000.00m, Juro = 15.00m, SaldoFinal = 1015.00m },
                new() { Mes = 2, SaldoInicial = 1015.00m, Juro = 15.23m, SaldoFinal = 1030.23m }
            }
        };

        _simulacaoRepositorio.ObterPorIdComEvolucoesAsync(simulacaoId, Arg.Any<CancellationToken>())
            .Returns(simulacao);

        // Act
        var resultado = await _handler.Handle(simulacaoId, cancellationToken);

        // Assert
        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Should().NotBeNull();
        resultado.Value.Id.Should().Be(simulacaoId);
        resultado.Value.ValorTotalFinal.Should().Be(1030.23m);
        resultado.Value.MemoriaCalculo.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_ComIdInexistente_DeveRetornarFalhaNaoEncontrado()
    {
        // Arrange
        int simulacaoId = 999;
        var cancellationToken = TestContext.Current.CancellationToken;
        _simulacaoRepositorio.ObterPorIdComEvolucoesAsync(simulacaoId, Arg.Any<CancellationToken>())
            .Returns((Simulacao?)null);

        // Act
        var resultado = await _handler.Handle(simulacaoId, cancellationToken);

        // Assert
        resultado.IsFailure.Should().BeTrue();
        resultado.Error.Should().NotBeNull();
        resultado.Error!.Type.Should().Be(global::CalculadoraEvolucaoSaldoEvo.Domain.Common.ErrorType.NotFound);
    }
}

