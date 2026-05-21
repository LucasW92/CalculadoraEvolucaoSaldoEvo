using CalculadoraEvolucaoSaldoEvo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CalculadoraEvolucaoSaldoEvo.Infrastructure.Persistence.Configurations;

public sealed class EvolucaoConfiguration : IEntityTypeConfiguration<Evolucao>
{
    public void Configure(EntityTypeBuilder<Evolucao> builder)
    {
        builder.ToTable("Evolucoes");

        builder.HasQueryFilter(x => !x.Simulacao.Deletado);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Mes)
            .IsRequired();

        builder.Property(x => x.SaldoInicial)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.Juro)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.SaldoFinal)
            .HasPrecision(18, 2)
            .IsRequired();
    }
}

