using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EF.Carrinho.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class BaseInicialCarrinho : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Carrinho",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClienteId = table.Column<Guid>(type: "uuid", nullable: true),
                    ValorTotal = table.Column<decimal>(type: "numeric", nullable: false),
                    TempoMedioPreparo = table.Column<int>(type: "integer", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carrinho", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ItensCarrinho",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ValorUnitario = table.Column<decimal>(type: "numeric", nullable: false),
                    Quantidade = table.Column<int>(type: "integer", nullable: false),
                    ProdutoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    TempoEstimadoPreparo = table.Column<int>(type: "integer", nullable: false),
                    CarrinhoId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItensCarrinho", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItensCarrinho_Carrinho_CarrinhoId",
                        column: x => x.CarrinhoId,
                        principalTable: "Carrinho",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IDX_Cliente",
                table: "Carrinho",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_ItensCarrinho_CarrinhoId",
                table: "ItensCarrinho",
                column: "CarrinhoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItensCarrinho");

            migrationBuilder.DropTable(
                name: "Carrinho");
        }
    }
}
