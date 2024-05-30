using EF.Core.Commons.Messages;
using EF.Core.Commons.Repository;
using EF.Cupons.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EF.Cupons.Infra;

public sealed class CupomDbContext : DbContext, IUnitOfWork
{
    public CupomDbContext(DbContextOptions<CupomDbContext> options) : base(options)
    {
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        ChangeTracker.AutoDetectChangesEnabled = false;
    }

    public DbSet<Cupom> Cupons { get; set; }
    public DbSet<CupomProduto> CupomProdutos { get; set; }

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

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CupomDbContext).Assembly);
        modelBuilder.Ignore<Event>();

        base.OnModelCreating(modelBuilder);
    }

    private void SetDates(IEnumerable<EntityEntry> entries)
    {
        foreach (var entry in entries
                     .Where(entry =>
                         entry.Entity.GetType().GetProperty("DataInicio") != null &&
                         entry.Entity.GetType().GetProperty("DataFim") != null))
        {
            var dtaInicio = (DateTime)entry.Property("DataInicio").CurrentValue!;
            var dtaFim = (DateTime)entry.Property("DataFim").CurrentValue!;

            entry.Property("DataInicio").CurrentValue = dtaInicio.ToUniversalTime();
            entry.Property("DataFim").CurrentValue = dtaFim.ToUniversalTime();
        }
    }
}