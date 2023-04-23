using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniERP.InventoryService.Migrations
{
    /// <inheritdoc />
    public partial class modifycview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ExpectedDate",
                table: "StockMovements",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.Sql(@"DROP VIEW ""AvailableInventoryView""");

            migrationBuilder.Sql(@"CREATE VIEW ""AvailableInventoryView"" AS
SELECT COALESCE(SUM(CASE WHEN m.""MovementType"" = 1 AND m.""MovementStatus"" = 2 THEN m.""Quantity""
                WHEN m.""MovementType"" = 2 AND m.""MovementStatus"" IN (1, 2) THEN -m.""Quantity""
                ELSE 0 END),0) AS ""Quantity"",
       i.""Status"",
       i.""ArticleId""
  FROM ""StockMovements"" AS m
INNER JOIN ""InventoryItems"" AS i
 ON (m.""InventoryId"" = i.""Id"") 
GROUP BY i.""ArticleId"", i.""Status""
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpectedDate",
                table: "StockMovements");

            migrationBuilder.Sql(@"DROP VIEW ""AvailableInventoryView""");

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
        }
    }
}
