using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameTradeZone.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ForumCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "ServicePrice",
                table: "Services",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ServiceLevel",
                table: "Services",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "PostInfos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "HiredServices",
                type: "nvarchar(max)",
                nullable: true);

            // Removed the duplicate addition of the 'Image' column
            // migrationBuilder.AddColumn<string>(
            //     name: "Image",
            //     table: "HiredServices",
            //     type: "nvarchar(max)",
            //     nullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "Experience",
                table: "AspNetUsers",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "ForumsCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForumsCategories", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ForumsCategories");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "PostInfos");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "HiredServices");

            // Removed the duplicate drop of the 'Image' column
            // migrationBuilder.DropColumn(
            //     name: "Image",
            //     table: "HiredServices");

            migrationBuilder.AlterColumn<decimal>(
                name: "ServicePrice",
                table: "Services",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "ServiceLevel",
                table: "Services",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Experience",
                table: "AspNetUsers",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);
        }
    }
}