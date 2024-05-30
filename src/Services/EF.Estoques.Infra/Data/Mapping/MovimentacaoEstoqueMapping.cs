using EF.Estoques.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EF.Estoques.Infra.Data.Mapping;

public class MovimentacaoEstoqueMapping : IEntityTypeConfiguration<MovimentacaoEstoque>
{
    public void Configure(EntityTypeBuilder<MovimentacaoEstoque> builder)
    {
        builder.ToTable("MovimentacoesEstoque");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.ProdutoId)
            .IsRequired();

        builder.Property(c => c.Quantidade)
            .IsRequired();

        builder.Property(c => c.DataLancamento)
            .IsRequired();

        builder.Property(c => c.TipoMovimentacao)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(c => c.OrigemMovimentacao)
            .IsRequired()
            .HasConversion<int>();

        builder.HasOne(c => c.Estoque)
            .WithMany(c => c.Movimentacoes);
    }
}