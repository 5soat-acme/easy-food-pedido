using EF.Pedidos.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EF.Pedidos.Infra.Data.Mapping;

public class ItemMapping : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.ToTable("ItensPedido");

        builder.HasKey(c => c.Id);

        builder.HasOne(c => c.Pedido)
            .WithMany(c => c.Itens);
    }
}