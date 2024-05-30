using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EF.Produtos.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class BaseInicialProdutos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Produtos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "varchar(100)", nullable: false),
                    ValorUnitario = table.Column<decimal>(type: "money", nullable: false),
                    Ativo = table.Column<bool>(type: "bool", nullable: false),
                    Categoria = table.Column<int>(type: "integer", nullable: false),
                    TempoPreparoEstimado = table.Column<int>(type: "integer", nullable: false),
                    Descricao = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produtos", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Produtos");
        }
    }
}
