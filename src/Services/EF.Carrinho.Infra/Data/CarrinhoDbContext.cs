using EF.Carrinho.Domain.Models;
using EF.Core.Commons.Messages;
using EF.Core.Commons.Repository;
using EF.Infra.Commons.Data;
using Microsoft.EntityFrameworkCore;

namespace EF.Carrinho.Infra.Data;

public sealed class CarrinhoDbContext : DbContext, IUnitOfWork
{
    public CarrinhoDbContext(DbContextOptions<CarrinhoDbContext> options) : base(options)
    {
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        ChangeTracker.AutoDetectChangesEnabled = false;
    }

    public DbSet<CarrinhoCliente>? Carrinhos { get; set; }
    public DbSet<Item>? Itens { get; set; }

    public async Task<bool> Commit()
    {
        DbContextExtension.SetDates(ChangeTracker.Entries());
        return await SaveChangesAsync() > 0;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CarrinhoDbContext).Assembly);
        modelBuilder.Ignore<Event>();

        foreach (var relationship in modelBuilder.Model.GetEntityTypes()
                     .SelectMany(e => e.GetForeignKeys()))
            relationship.DeleteBehavior = DeleteBehavior.Cascade;

        base.OnModelCreating(modelBuilder);
    }
}