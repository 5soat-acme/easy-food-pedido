using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using EF.Identidade.Domain.Models;

namespace EF.Identidade.Infra.Data.Mapping;

public class SolicitacaoExclusaoMapping : IEntityTypeConfiguration<SolicitacaoExclusao>
{
    public void Configure(EntityTypeBuilder<SolicitacaoExclusao> builder)
    {
        builder.ToTable("SolicitacaoExclusao");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.ClienteId)
            .IsRequired();

        builder.Property(p => p.Nome)
            .IsRequired();

        builder.OwnsOne(c => c.Endereco, tf =>
        {
            tf.Property(c => c.Rua)
                .IsRequired()
                .HasColumnName("Rua");

            tf.Property(c => c.Numero)
                .IsRequired()
                .HasColumnName("Numero");

            tf.Property(c => c.Bairro)
                .IsRequired()
                .HasColumnName("Bairro");

            tf.Property(c => c.Complemento)
                .HasColumnName("Complemento");

            tf.Property(c => c.Cidade)
                .IsRequired()
                .HasColumnName("Cidade");

            tf.Property(c => c.Estado)
                .IsRequired()
                .HasColumnName("Estado");

            tf.Property(c => c.Cep)
                .IsRequired()
                .HasColumnName("Cep");
        });

        builder.Property(p => p.NumeroTelefone)
           .IsRequired();
    }
}