using AwesomeAssertions;
using CalculadoraEvolucaoSaldoEvo.Domain.Common;
using CalculadoraEvolucaoSaldoEvo.Domain.Services;

namespace CalculadoraEvolucaoSaldoEvo.Application.UnitTests.Domain.Services;

public class TestesDoCalculadorJuros
{
    [Fact]
    public void Calcular_ComDadosValidos_DeveRetornarCalculoJurosCompostosCorreto()
    {
        // Arrange
        decimal valorInicial = 1000.00m;
        decimal taxaJurosMensal = 1.5m;
        int prazoMeses = 3;

        // Mês 1:
        // Saldo Inicial = 1000.00
        // Juro = 1000.00 * 0.015 = 15.00
        // Saldo Final = 1015.00
        //
        // Mês 2:
        // Saldo Inicial = 1015.00
        // Juro = 1015.00 * 0.015 = 15.225 -> 15.23 (arredondado para 2 casas decimais)
        // Saldo Final = 1030.23
        //
        // Mês 3:
        // Saldo Inicial = 1030.23
        // Juro = 1030.23 * 0.015 = 15.45345 -> 15.45 (arredondado)
        // Saldo Final = 1045.68

        // Act
        var resultado = CalculadorJuros.Calcular(valorInicial, taxaJurosMensal, prazoMeses);

        // Assert
        resultado.IsSuccess.Should().BeTrue();
        var calculo = resultado.Value!;
        calculo.ValorTotalFinal.Should().Be(1045.68m);
        calculo.TotalJuros.Should().Be(45.68m);
        calculo.Evolucoes.Should().HaveCount(3);

        // Validando mês 1
        calculo.Evolucoes[0].Mes.Should().Be(1);
        calculo.Evolucoes[0].SaldoInicial.Should().Be(1000.00m);
        calculo.Evolucoes[0].Juro.Should().Be(15.00m);
        calculo.Evolucoes[0].SaldoFinal.Should().Be(1015.00m);

        // Validando mês 2
        calculo.Evolucoes[1].Mes.Should().Be(2);
        calculo.Evolucoes[1].SaldoInicial.Should().Be(1015.00m);
        calculo.Evolucoes[1].Juro.Should().Be(15.23m);
        calculo.Evolucoes[1].SaldoFinal.Should().Be(1030.23m);

        // Validando mês 3
        calculo.Evolucoes[2].Mes.Should().Be(3);
        calculo.Evolucoes[2].SaldoInicial.Should().Be(1030.23m);
        calculo.Evolucoes[2].Juro.Should().Be(15.45m);
        calculo.Evolucoes[2].SaldoFinal.Should().Be(1045.68m);
    }

    [Fact]
    public void Calcular_ComTaxaJurosZero_DeveRetornarApenasValorInicialSemJuros()
    {
        // Arrange
        decimal valorInicial = 5000.00m;
        decimal taxaJurosMensal = 0.0m;
        int prazoMeses = 5;

        // Act
        var resultado = CalculadorJuros.Calcular(valorInicial, taxaJurosMensal, prazoMeses);

        // Assert
        resultado.IsSuccess.Should().BeTrue();
        var calculo = resultado.Value!;
        calculo.ValorTotalFinal.Should().Be(5000.00m);
        calculo.TotalJuros.Should().Be(0.00m);
        calculo.Evolucoes.Should().HaveCount(5);

        foreach (var item in calculo.Evolucoes)
        {
            item.Juro.Should().Be(0.00m);
            item.SaldoInicial.Should().Be(5000.00m);
            item.SaldoFinal.Should().Be(5000.00m);
        }
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Calcular_ComValorInicialInvalido_DeveRetornarFalhaDeValidacao(decimal valorInicial)
    {
        var resultado = CalculadorJuros.Calcular(valorInicial, 1.5m, 12);

        resultado.IsFailure.Should().BeTrue();
        resultado.Error.Should().NotBeNull();
        resultado.Error!.Type.Should().Be(ErrorType.Validation);
        resultado.Error.Code.Should().Be("Calculo.ValorInicialInvalido");
    }

    [Fact]
    public void Calcular_ComTaxaJurosNegativa_DeveRetornarFalhaDeValidacao()
    {
        var resultado = CalculadorJuros.Calcular(1000m, -0.01m, 12);

        resultado.IsFailure.Should().BeTrue();
        resultado.Error.Should().NotBeNull();
        resultado.Error!.Type.Should().Be(ErrorType.Validation);
        resultado.Error.Code.Should().Be("Calculo.TaxaJurosInvalida");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(601)]
    public void Calcular_ComPrazoInvalido_DeveRetornarFalhaDeValidacao(int prazoMeses)
    {
        var resultado = CalculadorJuros.Calcular(1000m, 1.5m, prazoMeses);

        resultado.IsFailure.Should().BeTrue();
        resultado.Error.Should().NotBeNull();
        resultado.Error!.Type.Should().Be(ErrorType.Validation);
        resultado.Error.Code.Should().Be("Calculo.PrazoMesesInvalido");
    }

    [Fact]
    public void Calcular_ComPrazoMaximo_DeveRetornarSucesso()
    {
        var resultado = CalculadorJuros.Calcular(1000m, 0m, 600);

        resultado.IsSuccess.Should().BeTrue();
        resultado.Value!.Evolucoes.Should().HaveCount(600);
    }
}
