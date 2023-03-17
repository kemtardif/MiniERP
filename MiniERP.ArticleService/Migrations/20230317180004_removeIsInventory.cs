using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniERP.ArticleService.Migrations
{
    /// <inheritdoc />
    public partial class removeIsInventory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsInventory",
                table: "Articles");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsInventory",
                table: "Articles",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
