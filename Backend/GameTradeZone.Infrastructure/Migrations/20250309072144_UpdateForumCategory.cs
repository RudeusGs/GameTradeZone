using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameTradeZone.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateForumCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IconClass",
                table: "ForumsCategories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PostCount",
                table: "ForumsCategories",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IconClass",
                table: "ForumsCategories");

            migrationBuilder.DropColumn(
                name: "PostCount",
                table: "ForumsCategories");
        }
    }
}
