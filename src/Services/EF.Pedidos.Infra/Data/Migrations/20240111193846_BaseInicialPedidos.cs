using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EF.Pedidos.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class BaseInicialPedidos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Pedidos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClienteId = table.Column<Guid>(type: "uuid", nullable: true),
                    Cpf = table.Column<string>(type: "varchar(11)", maxLength: 11, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ValorTotal = table.Column<decimal>(type: "numeric", nullable: false),
                    CupomId = table.Column<Guid>(type: "uuid", nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pedidos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ItensPedido",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ValorUnitario = table.Column<decimal>(type: "numeric", nullable: false),
                    Desconto = table.Column<decimal>(type: "numeric", nullable: true),
                    ValorFinal = table.Column<decimal>(type: "numeric", nullable: false),
                    Quantidade = table.Column<int>(type: "integer", nullable: false),
                    ProdutoId = table.Column<Guid>(type: "uuid", nullable: false),
                    PedidoId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItensPedido", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItensPedido_Pedidos_PedidoId",
                        column: x => x.PedidoId,
                        principalTable: "Pedidos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItensPedido_PedidoId",
                table: "ItensPedido",
                column: "PedidoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItensPedido");

            migrationBuilder.DropTable(
                name: "Pedidos");
        }
    }
}
