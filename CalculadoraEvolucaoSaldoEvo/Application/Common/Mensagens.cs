namespace CalculadoraEvolucaoSaldoEvo.Application.Common;

public static class Mensagens
{
    public const string ValorInicialInvalido = "O valor inicial deve ser maior que zero.";
    public const string TaxaJurosInvalida = "A taxa de juros mensal deve ser maior ou igual a zero.";
    public const string PrazoMesesInvalido = "O prazo deve ser maior que zero e menor ou igual a 600 meses.";
    public const string SimulacaoNaoEncontrada = "A simulação com o ID {0} não foi encontrada.";
    public const string SimulacaoCriadaSucesso = "Simulação calculada e salva com sucesso.";
    public const string ErroAoProcessarCalculo = "Ocorreu um erro ao processar o cálculo da simulação.";
}

