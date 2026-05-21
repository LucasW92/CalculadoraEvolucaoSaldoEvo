using AwesomeAssertions;
using CalculadoraEvolucaoSaldoEvo.Application.Abstractions.Data;
using CalculadoraEvolucaoSaldoEvo.Application.Abstractions.Persistence;
using CalculadoraEvolucaoSaldoEvo.Application.Features.Simulacoes.Criar;
using CalculadoraEvolucaoSaldoEvo.Domain.Common;
using CalculadoraEvolucaoSaldoEvo.Domain.Entities;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace CalculadoraEvolucaoSaldoEvo.Application.UnitTests.Features.Simulacoes;

public class TestesDoHandlerCriarSimulacao
{
    private readonly ISimulacaoRepositorio _simulacaoRepositorio = Substitute.For<ISimulacaoRepositorio>();
    private readonly IAppDbContext _unitOfWork = Substitute.For<IAppDbContext>();
    private readonly CriarSimulacaoHandler _handler;

    public TestesDoHandlerCriarSimulacao()
    {
        _handler = new CriarSimulacaoHandler(
            _simulacaoRepositorio,
            _unitOfWork,
            NullLogger<CriarSimulacaoHandler>.Instance
        );
    }

    [Fact]
    public async Task Handle_ComDadosValidos_DeveSalvarNoBanco()
    {
        // Arrange
        var request = new CriarSimulacaoRequest(1000.00m, 1.5m, 3);
        var cancellationToken = TestContext.Current.CancellationToken;

        _unitOfWork.SaveChangesAsync(cancellationToken).Returns(1);

        // Act
        var resultado = await _handler.Handle(request, cancellationToken);

        // Assert
        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Should().NotBeNull();
        resultado.Value.ValorTotalFinal.Should().Be(1045.68m);
        resultado.Value.TotalJuros.Should().Be(45.68m);
        resultado.Value.MemoriaCalculo.Should().HaveCount(3);

        // Verifica que adicionou no repositório
        _simulacaoRepositorio.Received(1).Adicionar(Arg.Is<Simulacao>(s =>
            s.ValorInicial == 1000.00m &&
            s.TaxaJurosMensal == 1.5m &&
            s.PrazoMeses == 3 &&
            s.ValorTotalFinal == 1045.68m &&
            s.TotalJuros == 45.68m &&
            s.Evolucoes.Count == 3
        ));

        await _unitOfWork.Received(1).SaveChangesAsync(cancellationToken);
    }

    [Fact]
    public async Task Handle_ComDadosInvalidos_DeveRetornarFalhaSemSalvar()
    {
        // Arrange
        var request = new CriarSimulacaoRequest(0m, 1.5m, 12);
        var cancellationToken = TestContext.Current.CancellationToken;

        // Act
        var resultado = await _handler.Handle(request, cancellationToken);

        // Assert
        resultado.IsFailure.Should().BeTrue();
        resultado.Error.Should().NotBeNull();
        resultado.Error!.Type.Should().Be(ErrorType.Validation);

        _simulacaoRepositorio.DidNotReceive().Adicionar(Arg.Any<Simulacao>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ComFalhaTecnicaAoSalvar_DevePropagarExcecao()
    {
        // Arrange
        var request = new CriarSimulacaoRequest(1000.00m, 1.5m, 3);
        var cancellationToken = TestContext.Current.CancellationToken;

        _unitOfWork
            .SaveChangesAsync(cancellationToken)
            .Returns(Task.FromException<int>(new InvalidOperationException("Banco indisponível.")));

        // Act
        Func<Task> act = () => _handler.Handle(request, cancellationToken);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Banco indisponível.");
    }
}
