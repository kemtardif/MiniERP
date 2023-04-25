using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniERP.InventoryService.Migrations
{
    /// <inheritdoc />
    public partial class yetanotherview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP VIEW ""AvailableInventoryView""");

            migrationBuilder.Sql(@"CREATE VIEW ""AvailableInventoryView"" AS
SELECT COALESCE(SUM(CASE WHEN m.""MovementType"" = 1 AND m.""MovementStatus"" = 2 THEN m.""Quantity""
                WHEN m.""MovementType"" = 2 AND m.""MovementStatus"" IN (1, 2) THEN -m.""Quantity""
                ELSE 0 END),0) AS ""Quantity"",
       i.""Status"",
       i.""ArticleId""
  FROM ""StockMovements"" AS m
RIGHT JOIN ""InventoryItems"" AS i
 ON (m.""ArticleId"" = i.""ArticleId"") 
GROUP BY i.""ArticleId"", i.""Status""
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP VIEW ""AvailableInventoryView""");

            migrationBuilder.Sql(@"CREATE VIEW ""AvailableInventoryView"" AS
SELECT COALESCE(SUM(CASE WHEN m.""MovementType"" = 1 AND m.""MovementStatus"" = 2 THEN m.""Quantity""
                WHEN m.""MovementType"" = 2 AND m.""MovementStatus"" IN (1, 2) THEN -m.""Quantity""
                ELSE 0 END),0) AS ""Quantity"",
       i.""Status"",
       i.""ArticleId""
  FROM ""StockMovements"" AS m
RIGHT JOIN ""InventoryItems"" AS i
 ON (m.""InventoryId"" = i.""Id"") 
GROUP BY i.""ArticleId"", i.""Status""
");
        }
    }
}
