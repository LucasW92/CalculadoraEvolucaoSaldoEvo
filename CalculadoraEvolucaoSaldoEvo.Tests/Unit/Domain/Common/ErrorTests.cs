using AwesomeAssertions;
using CalculadoraEvolucaoSaldoEvo.Domain.Common;

namespace CalculadoraEvolucaoSaldoEvo.Domain.UnitTests.Common;

public sealed class TestesDeErro
{
    [Fact]
    public void NaoEncontrado_DeveCriarErroComTipoNaoEncontrado()
    {
        var error = Error.NotFound("simulacao.nao-encontrada", "Simulação não encontrada.");

        error.Code.Should().Be("simulacao.nao-encontrada");
        error.Message.Should().Be("Simulação não encontrada.");
        error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public void Validacao_DeveCriarErroComTipoValidacao()
    {
        var error = Error.Validation("simulacao.invalida", "Dados inválidos.");

        error.Code.Should().Be("simulacao.invalida");
        error.Message.Should().Be("Dados inválidos.");
        error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public void Conflito_DeveCriarErroComTipoConflito()
    {
        var error = Error.Conflict("simulacao.conflito", "Conflito detectado.");

        error.Code.Should().Be("simulacao.conflito");
        error.Message.Should().Be("Conflito detectado.");
        error.Type.Should().Be(ErrorType.Conflict);
    }

    [Fact]
    public void Falha_DeveCriarErroComTipoFalha()
    {
        var error = Error.Failure("simulacao.falha", "Falha inesperada.");

        error.Code.Should().Be("simulacao.falha");
        error.Message.Should().Be("Falha inesperada.");
        error.Type.Should().Be(ErrorType.Failure);
    }
}