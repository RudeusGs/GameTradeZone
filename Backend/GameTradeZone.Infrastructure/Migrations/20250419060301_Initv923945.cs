using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameTradeZone.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initv923945 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Decription",
                table: "HiredServices",
                newName: "Decriptions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Decriptions",
                table: "HiredServices",
                newName: "Decription");
        }
    }
}
