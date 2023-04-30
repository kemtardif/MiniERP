using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniERP.InventoryService.Migrations
{
    /// <inheritdoc />
    public partial class changeMovemntRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockMovements_InventoryItems_ArticleId",
                table: "StockMovements");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_InventoryItems_ArticleId",
                table: "InventoryItems");

            migrationBuilder.RenameColumn(
                name: "ArticleId",
                table: "StockMovements",
                newName: "InventoryId");

            migrationBuilder.RenameIndex(
                name: "IX_StockMovements_ArticleId",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockMovements_InventoryItems_InventoryId",
                table: "StockMovements");

            migrationBuilder.RenameColumn(
                name: "InventoryId",
                table: "StockMovements",
                newName: "ArticleId");

            migrationBuilder.RenameIndex(
                name: "IX_StockMovements_InventoryId",
                table: "StockMovements",
                newName: "IX_StockMovements_ArticleId");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_InventoryItems_ArticleId",
                table: "InventoryItems",
                column: "ArticleId");

            migrationBuilder.AddForeignKey(
                name: "FK_StockMovements_InventoryItems_ArticleId",
                table: "StockMovements",
                column: "ArticleId",
                principalTable: "InventoryItems",
                principalColumn: "ArticleId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
