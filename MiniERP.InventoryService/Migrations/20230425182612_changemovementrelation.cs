using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniERP.InventoryService.Migrations
{
    /// <inheritdoc />
    public partial class changemovementrelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockMovements_InventoryItems_InventoryId",
                table: "StockMovements");

            migrationBuilder.RenameColumn(
                name: "InventoryId",
                table: "StockMovements",
                newName: "InventoryItemId");

            migrationBuilder.RenameIndex(
                name: "IX_StockMovements_InventoryId",
                table: "StockMovements",
                newName: "IX_StockMovements_InventoryItemId");

            migrationBuilder.AddColumn<int>(
                name: "ArticleId",
                table: "StockMovements",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_StockMovements_InventoryItems_InventoryItemId",
                table: "StockMovements",
                column: "InventoryItemId",
                principalTable: "InventoryItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockMovements_InventoryItems_InventoryItemId",
                table: "StockMovements");

            migrationBuilder.DropColumn(
                name: "ArticleId",
                table: "StockMovements");

            migrationBuilder.RenameColumn(
                name: "InventoryItemId",
                table: "StockMovements",
                newName: "InventoryId");

            migrationBuilder.RenameIndex(
                name: "IX_StockMovements_InventoryItemId",
                table: "StockMovements",
                newName: "IX_StockMovements_InventoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_StockMovements_InventoryItems_InventoryId",
                table: "StockMovements",
                column: "InventoryId",
                principalTable: "InventoryItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
