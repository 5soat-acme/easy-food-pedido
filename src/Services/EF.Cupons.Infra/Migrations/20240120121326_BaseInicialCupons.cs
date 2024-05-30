using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EF.Cupons.Infra.Migrations
{
    /// <inheritdoc />
    public partial class BaseInicialCupons : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cupons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DataInicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataFim = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CodigoCupom = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    PorcentagemDesconto = table.Column<decimal>(type: "numeric", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cupons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CupomProdutos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CupomId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProdutoId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CupomProdutos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CupomProdutos_Cupons_CupomId",
                        column: x => x.CupomId,
                        principalTable: "Cupons",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CupomProdutos_CupomId",
                table: "CupomProdutos",
                column: "CupomId");

            migrationBuilder.CreateIndex(
                name: "IX_Cupons_CodigoCupom",
                table: "Cupons",
                column: "CodigoCupom");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CupomProdutos");

            migrationBuilder.DropTable(
                name: "Cupons");
        }
    }
}
