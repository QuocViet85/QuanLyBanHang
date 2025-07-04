using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebBanHang.Migrations
{
    /// <inheritdoc />
    public partial class SetDb2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Taxes",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Taxes_UserId",
                table: "Taxes",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Taxes_AspNetUsers_UserId",
                table: "Taxes",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Taxes_AspNetUsers_UserId",
                table: "Taxes");

            migrationBuilder.DropIndex(
                name: "IX_Taxes_UserId",
                table: "Taxes");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Taxes");
        }
    }
}
