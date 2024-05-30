using EF.Cupons.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EF.Cupons.Infra.Data.Mapping;

public class CupomMapping : IEntityTypeConfiguration<Cupom>
{
    public void Configure(EntityTypeBuilder<Cupom> builder)
    {
        builder.ToTable("Cupons");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.DataInicio)
            .IsRequired();

        builder.Property(c => c.DataFim)
            .IsRequired();

        builder.Property(c => c.CodigoCupom)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(c => c.PorcentagemDesconto)
            .IsRequired();

        builder.Property(c => c.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.HasIndex(c => c.CodigoCupom);

        builder.HasMany(c => c.CupomProdutos)
            .WithOne(c => c.Cupom)
            .HasForeignKey(c => c.CupomId);
    }
}