using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Contoso.Api.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SaleOrder",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderNumber = table.Column<int>(nullable: false),
                    Status = table.Column<string>(maxLength: 16, nullable: false),
                    CreatedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()"),
                    CustomerName = table.Column<string>(maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleOrder", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SaleOrderItem",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Quantity = table.Column<int>(nullable: false),
                    TaxRate = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    UnitPriceExcludeTax = table.Column<decimal>(type: "money", nullable: false),
                    ProductId = table.Column<string>(maxLength: 128, nullable: false),
                    SaleOrderId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleOrderItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SaleOrderItem_SaleOrder_SaleOrderId",
                        column: x => x.SaleOrderId,
                        principalTable: "SaleOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderItem_SaleOrderId",
                table: "SaleOrderItem",
                column: "SaleOrderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SaleOrderItem");

            migrationBuilder.DropTable(
                name: "SaleOrder");
        }
    }
}
