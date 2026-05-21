using CalculadoraEvolucaoSaldoEvo.Application.Common;
using FluentValidation;

namespace CalculadoraEvolucaoSaldoEvo.Application.Features.Simulacoes.Criar;

public sealed class CriarSimulacaoValidator : AbstractValidator<CriarSimulacaoRequest>
{
    public CriarSimulacaoValidator()
    {
        RuleFor(x => x.ValorInicial)
            .GreaterThan(0)
            .WithMessage(Mensagens.ValorInicialInvalido);

        RuleFor(x => x.TaxaJurosMensal)
            .GreaterThanOrEqualTo(0)
            .WithMessage(Mensagens.TaxaJurosInvalida);

        RuleFor(x => x.PrazoMeses)
            .GreaterThan(0)
            .LessThanOrEqualTo(600)
            .WithMessage(Mensagens.PrazoMesesInvalido);
    }
}

