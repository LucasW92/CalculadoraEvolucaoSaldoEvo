using CalculadoraEvolucaoSaldoEvo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CalculadoraEvolucaoSaldoEvo.Infrastructure.Persistence.Configurations;

public sealed class SimulacaoConfiguration : IEntityTypeConfiguration<Simulacao>
{
    public void Configure(EntityTypeBuilder<Simulacao> builder)
    {
        builder.ToTable("Simulacoes");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ValorInicial)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.TaxaJurosMensal)
            .HasPrecision(5, 2)
            .IsRequired();

        builder.Property(x => x.PrazoMeses)
            .IsRequired();

        builder.Property(x => x.ValorTotalFinal)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.TotalJuros)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.HasMany(x => x.Evolucoes)
            .WithOne(x => x.Simulacao)
            .HasForeignKey(x => x.SimulacaoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

