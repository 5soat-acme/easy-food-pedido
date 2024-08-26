﻿// <auto-generated />
using System;
using EF.Identidade.Infra;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EF.Identidade.Infra.Migrations
{
    [DbContext(typeof(IdentidadeDbContext))]
    [Migration("20240720195431_BaseInicialIdentidade")]
    partial class BaseInicialIdentidade
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("EF.Identidade.Domain.Models.SolicitacaoExclusao", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("ClienteId")
                        .HasColumnType("uuid");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("NumeroTelefone")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("SolicitacaoExclusao", (string)null);
                });

            modelBuilder.Entity("EF.Identidade.Domain.Models.SolicitacaoExclusao", b =>
                {
                    b.OwnsOne("EF.Core.Commons.ValueObjects.Endereco", "Endereco", b1 =>
                        {
                            b1.Property<Guid>("SolicitacaoExclusaoId")
                                .HasColumnType("uuid");

                            b1.Property<string>("Bairro")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasColumnName("Bairro");

                            b1.Property<string>("Cep")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasColumnName("Cep");

                            b1.Property<string>("Cidade")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasColumnName("Cidade");

                            b1.Property<string>("Complemento")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasColumnName("Complemento");

                            b1.Property<string>("Estado")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasColumnName("Estado");

                            b1.Property<int>("Numero")
                                .HasColumnType("integer")
                                .HasColumnName("Numero");

                            b1.Property<string>("Rua")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasColumnName("Rua");

                            b1.HasKey("SolicitacaoExclusaoId");

                            b1.ToTable("SolicitacaoExclusao");

                            b1.WithOwner()
                                .HasForeignKey("SolicitacaoExclusaoId");
                        });

                    b.Navigation("Endereco")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
