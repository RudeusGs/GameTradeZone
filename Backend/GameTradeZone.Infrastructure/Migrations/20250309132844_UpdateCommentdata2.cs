using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameTradeZone.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCommentdata2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_CommentDatas_UserId",
                table: "CommentDatas",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_CommentDatas_AspNetUsers_UserId",
                table: "CommentDatas",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommentDatas_AspNetUsers_UserId",
                table: "CommentDatas");

            migrationBuilder.DropIndex(
                name: "IX_CommentDatas_UserId",
                table: "CommentDatas");
        }
    }
}
