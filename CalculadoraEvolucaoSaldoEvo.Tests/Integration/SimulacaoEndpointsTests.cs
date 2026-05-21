using System.Data.Common;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using AwesomeAssertions;
using CalculadoraEvolucaoSaldoEvo.Application.Features.Simulacoes.Criar;
using CalculadoraEvolucaoSaldoEvo.Application.Features.Simulacoes.Consultar;
using CalculadoraEvolucaoSaldoEvo.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CalculadoraEvolucaoSaldoEvo.Application.IntegrationTests;

public sealed class TestesDosEndpointsDeSimulacao
{
    [Fact]
    public async Task CriarSimulacao_ComPayloadValido_DeveRetornar201ECabecalhoLocation()
    {
        using var factory = new SimulacaoApiFactory();
        using var client = factory.CreateClient();
        var cancellationToken = TestContext.Current.CancellationToken;

        var response = await client.PostAsJsonAsync(
            "/api/simulacoes/",
            new CriarSimulacaoRequest(1000m, 1.5m, 3),
            cancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();

        var body = await response.Content.ReadFromJsonAsync<CriarSimulacaoResponse>(cancellationToken);
        body.Should().NotBeNull();
        body!.Id.Should().BeGreaterThan(0);
        body.ValorTotalFinal.Should().Be(1045.68m);
        body.MemoriaCalculo.Should().HaveCount(3);

        response.Headers.Location!.ToString().Should().Contain($"/api/simulacoes/{body.Id}");
    }

    [Fact]
    public async Task ConsultarSimulacao_ComIdExistente_DeveRetornar200ComMemoriaDeCalculo()
    {
        using var factory = new SimulacaoApiFactory();
        using var client = factory.CreateClient();
        var cancellationToken = TestContext.Current.CancellationToken;

        var respostaCriacao = await client.PostAsJsonAsync(
            "/api/simulacoes/",
            new CriarSimulacaoRequest(1500m, 2m, 2),
            cancellationToken);

        respostaCriacao.StatusCode.Should().Be(HttpStatusCode.Created);

        var simulacaoCriada = await respostaCriacao.Content.ReadFromJsonAsync<CriarSimulacaoResponse>(cancellationToken);
        simulacaoCriada.Should().NotBeNull();

        var respostaConsulta = await client.GetAsync($"/api/simulacoes/{simulacaoCriada!.Id}", cancellationToken);

        respostaConsulta.StatusCode.Should().Be(HttpStatusCode.OK);

        var simulacaoConsultada = await respostaConsulta.Content.ReadFromJsonAsync<ConsultarSimulacaoResponse>(cancellationToken);
        simulacaoConsultada.Should().NotBeNull();
        simulacaoConsultada!.Id.Should().Be(simulacaoCriada.Id);
        simulacaoConsultada.MemoriaCalculo.Should().HaveCount(2);
        simulacaoConsultada.TotalJuros.Should().Be(60.60m);
    }

    [Fact]
    public async Task CriarSimulacao_ComJsonVazio_DeveRetornar400()
    {
        using var factory = new SimulacaoApiFactory();
        using var client = factory.CreateClient();
        var cancellationToken = TestContext.Current.CancellationToken;

        var response = await client.PostAsJsonAsync("/api/simulacoes/", new { }, cancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CriarSimulacao_ComCampoNumericoNulo_DeveRetornar400()
    {
        using var factory = new SimulacaoApiFactory();
        using var client = factory.CreateClient();
        var cancellationToken = TestContext.Current.CancellationToken;

        var content = new StringContent(
            """
            {
              "valorInicial": null,
              "taxaJurosMensal": 1.5,
              "prazoMeses": 12
            }
            """,
            Encoding.UTF8,
            "application/json");

        var response = await client.PostAsync("/api/simulacoes/", content, cancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData("valorInicial", "\"1000\"")]
    [InlineData("taxaJurosMensal", "\"1.5\"")]
    [InlineData("prazoMeses", "\"12\"")]
    public async Task CriarSimulacao_ComCampoNumericoComoString_DeveRetornar400(string campo, string valorJson)
    {
        using var factory = new SimulacaoApiFactory();
        using var client = factory.CreateClient();
        var cancellationToken = TestContext.Current.CancellationToken;

        var json = $$"""
        {
          "valorInicial": 1000,
          "taxaJurosMensal": 1.5,
          "prazoMeses": 12
        }
        """;

        json = campo switch
        {
            "valorInicial" => json.Replace("1000", valorJson, StringComparison.Ordinal),
            "taxaJurosMensal" => json.Replace("1.5", valorJson, StringComparison.Ordinal),
            "prazoMeses" => json.Replace("12", valorJson, StringComparison.Ordinal),
            _ => throw new ArgumentOutOfRangeException(nameof(campo), campo, null)
        };

        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync("/api/simulacoes/", content, cancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CriarSimulacao_ComCorpoApenasEspacos_DeveRetornar400()
    {
        using var factory = new SimulacaoApiFactory();
        using var client = factory.CreateClient();
        var cancellationToken = TestContext.Current.CancellationToken;
        using var content = new StringContent("   ", Encoding.UTF8, "application/json");

        var response = await client.PostAsync("/api/simulacoes/", content, cancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ConsultarSimulacao_ComIdInexistente_DeveRetornar404()
    {
        using var factory = new SimulacaoApiFactory();
        using var client = factory.CreateClient();
        var cancellationToken = TestContext.Current.CancellationToken;

        var response = await client.GetAsync("/api/simulacoes/999", cancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task OpenApi_DeveDocumentarSumariosDescricoesEPrincipaisRespostas()
    {
        using var factory = new SimulacaoApiFactory();
        using var client = factory.CreateClient();
        var cancellationToken = TestContext.Current.CancellationToken;

        var response = await client.GetAsync("/openapi/v1.json", cancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync(cancellationToken));
        var paths = document.RootElement.GetProperty("paths");

        var propriedadeCriacao = paths.EnumerateObject()
            .First(path => path.Name.StartsWith("/api/simulacoes", StringComparison.Ordinal) && path.Value.TryGetProperty("post", out _));

        var operacaoCriacao = propriedadeCriacao.Value.GetProperty("post");
        operacaoCriacao.GetProperty("summary").GetString().Should().Contain("simulação");
        operacaoCriacao.GetProperty("description").GetString().Should().Contain("memória de cálculo");
        operacaoCriacao.GetProperty("responses").TryGetProperty("201", out _).Should().BeTrue();
        operacaoCriacao.GetProperty("responses").TryGetProperty("400", out _).Should().BeTrue();
        operacaoCriacao.GetProperty("responses").TryGetProperty("500", out _).Should().BeTrue();

        var schemas = document.RootElement.GetProperty("components").GetProperty("schemas");
        var schemaCriacao = schemas.GetProperty("CriarSimulacaoRequest");
        schemaCriacao.GetProperty("properties").GetProperty("valorInicial").GetProperty("type").GetString().Should().Be("number");
        schemaCriacao.GetProperty("properties").GetProperty("taxaJurosMensal").GetProperty("type").GetString().Should().Be("number");
        schemaCriacao.GetProperty("properties").GetProperty("prazoMeses").GetProperty("type").GetString().Should().Be("integer");

        var propriedadeConsulta = paths.EnumerateObject()
            .First(path => path.Name.Contains("{id}", StringComparison.Ordinal) && path.Value.TryGetProperty("get", out _));

        var operacaoConsulta = propriedadeConsulta.Value.GetProperty("get");
        operacaoConsulta.GetProperty("summary").GetString().Should().Contain("Consulta");
        var descricaoConsulta = operacaoConsulta.GetProperty("description").GetString();
        descricaoConsulta.Should().NotBeNullOrWhiteSpace();
        descricaoConsulta.Should().MatchRegex("(?i)(memória de cálculo|evolução|simulação)");
        operacaoConsulta.GetProperty("responses").TryGetProperty("200", out _).Should().BeTrue();
        operacaoConsulta.GetProperty("responses").TryGetProperty("404", out _).Should().BeTrue();
    }

    private sealed class SimulacaoApiFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Development");
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<DbContextOptions<AppDbContext>>();
                services.RemoveAll<DbConnection>();

                services.AddSingleton<DbConnection>(_ =>
                {
                    var connection = new SqliteConnection("Data Source=:memory:");
                    connection.Open();
                    return connection;
                });

                services.AddDbContext<AppDbContext>((serviceProvider, options) =>
                {
                    var connection = serviceProvider.GetRequiredService<DbConnection>();
                    var interceptor = serviceProvider.GetRequiredService<SoftDeleteInterceptor>();

                    options.UseSqlite(connection)
                        .AddInterceptors(interceptor);
                });
            });
        }
    }
}
