using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniERP.ArticleService.Migrations
{
    /// <inheritdoc />
    public partial class StockColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AutoOrder",
                table: "Articles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "AutoQuantity",
                table: "Articles",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "AutoTreshold",
                table: "Articles",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "MaxQuantity",
                table: "Articles",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AutoOrder",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "AutoQuantity",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "AutoTreshold",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "MaxQuantity",
                table: "Articles");
        }
    }
}
