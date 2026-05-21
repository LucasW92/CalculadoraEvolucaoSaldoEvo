using AwesomeAssertions;
using CalculadoraEvolucaoSaldoEvo.Domain.Common;

namespace CalculadoraEvolucaoSaldoEvo.Domain.UnitTests.Common;

public sealed class TestesDeResultado
{
    [Fact]
    public void SucessoSemValor_DeveCriarResultadoSemErro()
    {
        var result = Result.Success();

        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Error.Should().BeNull();
    }

    [Fact]
    public void FalhaSemValor_DeveCriarResultadoComErro()
    {
        var error = Error.Failure("resultado.falha", "Falha de teste.");

        var result = Result.Failure(error);

        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void SucessoComValor_DeveCriarResultadoTipado()
    {
        var result = Result.Success(new ResultadoFake("ok"));

        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Value.Should().BeEquivalentTo(new ResultadoFake("ok"));
        result.Error.Should().BeNull();
    }

    [Fact]
    public void FalhaComValor_DeveCriarResultadoTipadoComErro()
    {
        var error = Error.Failure("resultado.tipado.falha", "Falha tipada.");

        var result = Result.Failure<ResultadoFake>(error);

        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Value.Should().BeNull();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void ConstrutorBase_DeveRejeitarSucessoComErro()
    {
        var act = () => new ResultFake(true, Error.Failure("resultado.invalido", "Não deveria aceitar erro."));

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("A successful result cannot have an error.");
    }

    [Fact]
    public void ConstrutorBase_DeveRejeitarFalhaSemErro()
    {
        var act = () => new ResultFake(false, null);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("A failed result must have an error.");
    }

    private sealed record ResultadoFake(string Valor);

    private sealed class ResultFake(bool isSuccess, Error? error) : Result(isSuccess, error);
}