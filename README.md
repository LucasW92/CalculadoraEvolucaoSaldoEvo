# Calculadora Evolucao Saldo Evo

API em ASP.NET Core para simular a evolucao de saldo com juros compostos, persistir a simulacao e consultar a memoria de calculo por identificador.

## Funcionalidades

- criar simulacao com `valorInicial`, `taxaJurosMensal` e `prazoMeses`
- persistir os dados da simulacao e a memoria de calculo
- consultar simulacao por identificador
- expor contrato OpenAPI
- manter cobertura de testes acima de 80%

## Requisitos

- .NET SDK 10

## Executar a API

```powershell
dotnet run --project .\CalculadoraEvolucaoSaldoEvo\CalculadoraEvolucaoSaldoEvo.csproj
```

Em ambiente de desenvolvimento, a documentacao fica disponivel em:

- `http://localhost:5035/scalar/v1`
- `http://localhost:5035/openapi/v1.json`

## Executar os testes

```powershell
dotnet test .\CalculadoraEvolucaoSaldoEvo.Tests\CalculadoraEvolucaoSaldoEvo.Tests.csproj
```

## Endpoints principais

- `POST /api/simulacoes/`
- `GET /api/simulacoes/{id}`

## Persistencia

- SQLite para a aplicacao
- migration automatica em desenvolvimento
