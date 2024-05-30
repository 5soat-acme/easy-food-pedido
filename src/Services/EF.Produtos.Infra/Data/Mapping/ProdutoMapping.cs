using EF.Produtos.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EF.Produtos.Infra.Data.Mapping;

public class ProdutoMapping : IEntityTypeConfiguration<Produto>
{
    public void Configure(EntityTypeBuilder<Produto> builder)
    {
        builder.ToTable("Produtos");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Nome)
            .IsRequired()
            .HasColumnType("varchar(100)");

        builder.Property(p => p.ValorUnitario)
            .IsRequired()
            .HasColumnType("money");

        builder.Property(p => p.Ativo)
            .IsRequired()
            .HasColumnType("bool");

        builder.Property(p => p.Categoria)
            .IsRequired()
            .HasColumnType("integer");
    }
}