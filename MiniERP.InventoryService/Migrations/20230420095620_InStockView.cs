using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniERP.InventoryService.Migrations
{
    /// <inheritdoc />
    public partial class InStockView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "InventoryItems");

            migrationBuilder.Sql(@"CREATE VIEW ""AvailableInventoryView"" AS
SELECT SUM(CASE WHEN m.""MovementType"" = 1 AND m.""MovementStatus"" = 2 THEN m.""Quantity""
                WHEN m.""MovementType"" = 2 AND m.""MovementStatus"" IN (1, 2) THEN -m.""Quantity""
                ELSE 0 END) AS Quantity,
       i.""Status"",
       i.""ArticleId""
  FROM ""StockMovements"" AS m
INNER JOIN ""InventoryItems"" AS i
 ON (m.""InventoryId"" = i.""Id"") 
GROUP BY i.""ArticleId"", i.""Status""
");

            migrationBuilder.Sql(@"CREATE VIEW ""PendingInventoryView"" AS
SELECT SUM(CASE WHEN m.""MovementType"" = 1 THEN m.""Quantity"" ELSE -m.""Quantity"" END) AS Quantity,
       i.""Status"",
       i.""ArticleId""
  FROM ""StockMovements"" AS m
INNER JOIN ""InventoryItems"" AS i
 ON (m.""InventoryId"" = i.""Id"") 
 WHERE m.""MovementStatus"" = 1 
GROUP BY i.""ArticleId"", i.""Status""");


        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Quantity",
                table: "InventoryItems",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.Sql(@"DROP VIEW ""AvailableInventoryView""");

            migrationBuilder.Sql(@"DROP VIEW ""PendingInventoryView""");
        }
    }
}
