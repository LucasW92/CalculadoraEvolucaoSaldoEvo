using CalculadoraEvolucaoSaldoEvo.Application.Abstractions.Data;
using CalculadoraEvolucaoSaldoEvo.Domain.Common;
using CalculadoraEvolucaoSaldoEvo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CalculadoraEvolucaoSaldoEvo.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IAppDbContext
{
    public DbSet<Simulacao> Simulacoes { get; set; } = default!;
    public DbSet<Evolucao> Evolucoes { get; set; } = default!;
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        builder.ApplySoftDeleteQueryFilter();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditableEntities();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateAuditableEntities()
    {
        var entries = ChangeTracker.Entries<AuditableEntity>();
        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CriadoEm = DateTimeOffset.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.ModificadoEm = DateTimeOffset.UtcNow;
                    break;
            }
        }
    }
}

