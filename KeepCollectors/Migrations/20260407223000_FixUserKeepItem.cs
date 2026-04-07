using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KeepCollectors.Migrations
{
    /// <inheritdoc />
    public partial class FixUserKeepItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserKeepItems_AspNetUsers_UserID",
                table: "UserKeepItems");

            migrationBuilder.DropForeignKey(
                name: "FK_UserKeepItems_Products_ProductID",
                table: "UserKeepItems");

            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "UserKeepItems",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserKeepItems_UserID",
                table: "UserKeepItems",
                newName: "IX_UserKeepItems_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserKeepItems_AspNetUsers_UserId",
                table: "UserKeepItems",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserKeepItems_Products_ProductID",
                table: "UserKeepItems",
                column: "ProductID",
                principalTable: "Products",
                principalColumn: "ProductID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserKeepItems_AspNetUsers_UserId",
                table: "UserKeepItems");

            migrationBuilder.DropForeignKey(
                name: "FK_UserKeepItems_Products_ProductID",
                table: "UserKeepItems");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "UserKeepItems",
                newName: "UserID");

            migrationBuilder.RenameIndex(
                name: "IX_UserKeepItems_UserId",
                table: "UserKeepItems",
                newName: "IX_UserKeepItems_UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_UserKeepItems_AspNetUsers_UserID",
                table: "UserKeepItems",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserKeepItems_Products_ProductID",
                table: "UserKeepItems",
                column: "ProductID",
                principalTable: "Products",
                principalColumn: "ProductID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
