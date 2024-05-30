using EF.Carrinho.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EF.Carrinho.Infra.Data.Mapping;

public class ItemMapping : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.ToTable("ItensCarrinho");

        builder.HasKey(c => c.Id);

        builder.HasOne(c => c.Carrinho)
            .WithMany(c => c.Itens);
    }
}