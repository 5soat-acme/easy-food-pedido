using EF.Core.Commons.Messages;
using EF.Core.Commons.Repository;
using EF.Infra.Commons.Data;
using EF.Infra.Commons.EventBus;
using EF.Pedidos.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace EF.Pedidos.Infra.Data;

public sealed class PedidoDbContext : DbContext, IUnitOfWork
{
    private readonly IEventBus _bus;

    public PedidoDbContext(DbContextOptions<PedidoDbContext> options, IEventBus bus) : base(options)
    {
        _bus = bus;
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        ChangeTracker.AutoDetectChangesEnabled = false;
    }

    public DbSet<Pedido>? Pedidos { get; set; }
    public DbSet<Item>? Itens { get; set; }

    public async Task<bool> Commit()
    {
        DbContextExtension.SetDates(ChangeTracker.Entries());
        await _bus.PublishEvents(this);
        return await SaveChangesAsync() > 0;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        foreach (var relationship in modelBuilder.Model.GetEntityTypes()
                     .SelectMany(e => e.GetForeignKeys()))
            relationship.DeleteBehavior = DeleteBehavior.ClientSetNull;

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PedidoDbContext).Assembly);
        modelBuilder.Ignore<Event>();

        base.OnModelCreating(modelBuilder);
    }
}