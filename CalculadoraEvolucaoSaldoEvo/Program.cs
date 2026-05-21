using CalculadoraEvolucaoSaldoEvo.Api.Endpoints;
using CalculadoraEvolucaoSaldoEvo.Api.Extensions;
using CalculadoraEvolucaoSaldoEvo.Application;
using CalculadoraEvolucaoSaldoEvo.Infrastructure;
using CalculadoraEvolucaoSaldoEvo.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Serilog;
using System.Text.Json.Serialization;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Serilog
    builder.Host.UseSerilog((context, loggerConfiguration) =>
        loggerConfiguration.ReadFrom.Configuration(context.Configuration));

    // Application & Infrastructure
    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);

    // Global exception handling
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

    // OpenAPI
    builder.Services.AddOpenApi(options =>
    {
        options.AddDocumentTransformer((document, _, _) =>
        {
            var info = document.Info ?? new Microsoft.OpenApi.OpenApiInfo();
            info.Title = "Calculadora Evolução Saldo API";
            info.Description = "API para cálculo de evolução de saldo";
            document.Info = info;
            return Task.CompletedTask;
        });
    });

    // ProblemDetails
    builder.Services.AddProblemDetails();

    // JSON
    builder.Services.ConfigureHttpJsonOptions(options =>
    {
        options.SerializerOptions.NumberHandling = JsonNumberHandling.Strict;
    });

    // Health checks
    builder.Services.AddHealthChecks();

    var app = builder.Build();

    // Global exception handler
    app.UseExceptionHandler();
    app.UseStatusCodePages();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference(options =>
        {
            options.WithTitle("Calculadora Evolução Saldo API");
            options.WithTheme(ScalarTheme.BluePlanet);
            options.WithDefaultHttpClient(ScalarTarget.Shell, ScalarClient.Curl);
        });
    }

    app.UseSerilogRequestLogging();

    // Map endpoints
    app.MapSimulacaoEndpoints();

    // Health checks
    app.MapHealthChecks("/health");

    // Migrate database in development
    if (app.Environment.IsDevelopment())
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await context.Database.MigrateAsync();
    }

    await app.RunAsync();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
    throw;
}
finally
{
    await Log.CloseAndFlushAsync();
}