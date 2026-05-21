using AwesomeAssertions;
using NetArchTest.Rules;

namespace CalculadoraEvolucaoSaldoEvo.Architecture.Tests;

public sealed class TestesDeArquitetura
{
    private static readonly System.Reflection.Assembly ApplicationAssembly = typeof(CalculadoraEvolucaoSaldoEvo.Application.DependencyInjection).Assembly;

    private const string DomainNamespace = "CalculadoraEvolucaoSaldoEvo.Domain";
    private const string ApplicationNamespace = "CalculadoraEvolucaoSaldoEvo.Application";
    private const string InfrastructureNamespace = "CalculadoraEvolucaoSaldoEvo.Infrastructure";
    private const string ApiNamespace = "CalculadoraEvolucaoSaldoEvo.Api";

    [Fact]
    public void Dominio_NaoDeveDependerDaAplicacao()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .That()
            .ResideInNamespace(DomainNamespace)
            .ShouldNot()
            .HaveDependencyOn(ApplicationNamespace)
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Dominio_NaoDeveDependerDaInfraestrutura()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .That()
            .ResideInNamespace(DomainNamespace)
            .ShouldNot()
            .HaveDependencyOn(InfrastructureNamespace)
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Dominio_NaoDeveDependerDaApi()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .That()
            .ResideInNamespace(DomainNamespace)
            .ShouldNot()
            .HaveDependencyOn(ApiNamespace)
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Aplicacao_NaoDeveDependerDaInfraestrutura()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .That()
            .ResideInNamespace(ApplicationNamespace)
            .ShouldNot()
            .HaveDependencyOn(InfrastructureNamespace)
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Aplicacao_NaoDeveDependerDaApi()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .That()
            .ResideInNamespace(ApplicationNamespace)
            .ShouldNot()
            .HaveDependencyOn(ApiNamespace)
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Infraestrutura_NaoDeveDependerDaApi()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .That()
            .ResideInNamespace(InfrastructureNamespace)
            .ShouldNot()
            .HaveDependencyOn(ApiNamespace)
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Handlers_DevemSerSealed()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .That()
            .ResideInNamespace(ApplicationNamespace)
            .And()
            .HaveNameEndingWith("Handler")
            .Should()
            .BeSealed()
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Validators_DevemSerSealed()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .That()
            .ResideInNamespace(ApplicationNamespace)
            .And()
            .HaveNameEndingWith("Validator")
            .Should()
            .BeSealed()
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }
}
