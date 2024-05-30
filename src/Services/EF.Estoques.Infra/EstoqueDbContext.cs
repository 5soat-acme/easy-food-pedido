using EF.Core.Commons.Messages;
using EF.Core.Commons.Repository;
using EF.Estoques.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EF.Estoques.Infra;

public sealed class EstoqueDbContext : DbContext, IUnitOfWork
{
    public EstoqueDbContext(DbContextOptions<EstoqueDbContext> options) : base(options)
    {
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        ChangeTracker.AutoDetectChangesEnabled = false;
    }

    public DbSet<Estoque> Estoques { get; set; }
    public DbSet<MovimentacaoEstoque> MovimentacaoEstoques { get; set; }

    public async Task<bool> Commit()
    {
        SetDates(ChangeTracker.Entries());
        return await SaveChangesAsync() > 0;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        foreach (var relationship in modelBuilder.Model.GetEntityTypes()
                     .SelectMany(e => e.GetForeignKeys()))
            relationship.DeleteBehavior = DeleteBehavior.ClientSetNull;

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EstoqueDbContext).Assembly);
        modelBuilder.Ignore<Event>();

        base.OnModelCreating(modelBuilder);
    }

    private void SetDates(IEnumerable<EntityEntry> entries)
    {
        foreach (var entry in entries
                     .Where(entry =>
                         entry.Entity.GetType().GetProperty("DataLancamento") != null))
        {
            var dataLancamento = (DateTime)entry.Property("DataLancamento").CurrentValue!;

            entry.Property("DataLancamento").CurrentValue = dataLancamento.ToUniversalTime();
        }
    }
}