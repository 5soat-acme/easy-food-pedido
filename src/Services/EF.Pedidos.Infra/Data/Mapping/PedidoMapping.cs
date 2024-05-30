using EF.Core.Commons.ValueObjects;
using EF.Pedidos.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EF.Pedidos.Infra.Data.Mapping;

public class PedidoMapping : IEntityTypeConfiguration<Pedido>
{
    public void Configure(EntityTypeBuilder<Pedido> builder)
    {
        builder.ToTable("Pedidos");

        builder.HasKey(c => c.Id);

        builder.OwnsOne(c => c.Cpf, tf =>
        {
            tf.Property(c => c.Numero)
                .HasMaxLength(Cpf.CpfMaxLength)
                .HasColumnName("Cpf")
                .HasColumnType($"varchar({Cpf.CpfMaxLength})");
        });

        builder.HasMany(c => c.Itens)
            .WithOne(c => c.Pedido)
            .HasForeignKey(c => c.PedidoId);
    }
}