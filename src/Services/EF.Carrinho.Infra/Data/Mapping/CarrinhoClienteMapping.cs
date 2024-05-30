using EF.Carrinho.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EF.Carrinho.Infra.Data.Mapping;

public class CarrinhoClienteMapping : IEntityTypeConfiguration<CarrinhoCliente>
{
    public void Configure(EntityTypeBuilder<CarrinhoCliente> builder)
    {
        builder.ToTable("Carrinho");

        builder.HasKey(c => c.Id);

        builder.HasIndex(c => c.ClienteId)
            .HasName("IDX_Cliente");

        builder.HasMany(c => c.Itens)
            .WithOne(c => c.Carrinho)
            .HasForeignKey(c => c.CarrinhoId);
    }
}