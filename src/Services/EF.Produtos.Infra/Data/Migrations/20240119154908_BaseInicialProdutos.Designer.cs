﻿// <auto-generated />
using System;
using EF.Produtos.Infra.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EF.Produtos.Infra.Data.Migrations
{
    [DbContext(typeof(ProdutoDbContext))]
    [Migration("20240119154908_BaseInicialProdutos")]
    partial class BaseInicialProdutos
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("EF.Produtos.Domain.Models.Produto", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<bool>("Ativo")
                        .HasColumnType("bool");

                    b.Property<int>("Categoria")
                        .HasColumnType("integer");

                    b.Property<string>("Descricao")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasColumnType("varchar(100)");

                    b.Property<int>("TempoPreparoEstimado")
                        .HasColumnType("integer");

                    b.Property<decimal>("ValorUnitario")
                        .HasColumnType("money");

                    b.HasKey("Id");

                    b.ToTable("Produtos", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
