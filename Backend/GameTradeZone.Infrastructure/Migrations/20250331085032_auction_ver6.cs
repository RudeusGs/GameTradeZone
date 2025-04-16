using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameTradeZone.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class auction_ver6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EndDateTime",
                table: "Auctions",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDateTime",
                table: "Auctions");
        }
    }
}
