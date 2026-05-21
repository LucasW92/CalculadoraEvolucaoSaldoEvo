using AwesomeAssertions;
using CalculadoraEvolucaoSaldoEvo.Application.Features.Simulacoes.Criar;

namespace CalculadoraEvolucaoSaldoEvo.Application.UnitTests.Features.Simulacoes;

public class TestesDoValidadorCriarSimulacao
{
    private readonly CriarSimulacaoValidator _validator = new();

    [Fact]
    public void Validar_ComDadosValidos_DevePassarNaValidacao()
    {
        // Arrange
        var request = new CriarSimulacaoRequest(1000.00m, 1.5m, 12);

        // Act
        var resultado = _validator.Validate(request);

        // Assert
        resultado.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validar_ComPrazoMaximo_DevePassarNaValidacao()
    {
        // Arrange
        var request = new CriarSimulacaoRequest(1000.00m, 0m, 600);

        // Act
        var resultado = _validator.Validate(request);

        // Assert
        resultado.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-100)]
    public void Validar_ComValorInicialInvalido_DeveFalhar(decimal valorInicial)
    {
        // Arrange
        var request = new CriarSimulacaoRequest(valorInicial, 1.5m, 12);

        // Act
        var resultado = _validator.Validate(request);

        // Assert
        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().Contain(e => e.PropertyName == nameof(CriarSimulacaoRequest.ValorInicial));
    }

    [Fact]
    public void Validar_ComTaxaJurosNegativa_DeveFalhar()
    {
        // Arrange
        var request = new CriarSimulacaoRequest(1000.00m, -0.5m, 12);

        // Act
        var resultado = _validator.Validate(request);

        // Assert
        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().Contain(e => e.PropertyName == nameof(CriarSimulacaoRequest.TaxaJurosMensal));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    [InlineData(601)]
    public void Validar_ComPrazoInvalido_DeveFalhar(int prazoMeses)
    {
        // Arrange
        var request = new CriarSimulacaoRequest(1000.00m, 1.5m, prazoMeses);

        // Act
        var resultado = _validator.Validate(request);

        // Assert
        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().Contain(e => e.PropertyName == nameof(CriarSimulacaoRequest.PrazoMeses));
    }
}
