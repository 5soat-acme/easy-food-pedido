using EF.Estoques.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EF.Estoques.Infra.Data.Mapping;

public class EstoqueMapping : IEntityTypeConfiguration<Estoque>
{
    public void Configure(EntityTypeBuilder<Estoque> builder)
    {
        builder.ToTable("Estoques");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.ProdutoId)
            .IsRequired();

        builder.Property(c => c.Quantidade)
            .IsRequired();

        builder.HasIndex(c => c.ProdutoId)
            .IsUnique();

        builder.HasMany(c => c.Movimentacoes)
            .WithOne(c => c.Estoque)
            .HasForeignKey(c => c.EstoqueId);
    }
}