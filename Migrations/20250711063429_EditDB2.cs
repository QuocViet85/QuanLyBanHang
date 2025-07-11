using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebBanHang.Migrations
{
    /// <inheritdoc />
    public partial class EditDB2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Orders_OrderId",
                table: "OrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_Product",
                table: "OrderDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderDetails",
                table: "OrderDetails");

            migrationBuilder.RenameColumn(
                name: "TotalBeforeTax",
                table: "Orders",
                newName: "TotalBeforeDefaultTax");

            migrationBuilder.RenameColumn(
                name: "TotalAfterTax",
                table: "Orders",
                newName: "TotalAfterDefaultTax");

            migrationBuilder.RenameColumn(
                name: "Taxes",
                table: "OrderDetails",
                newName: "PrivateTaxes");

            migrationBuilder.RenameColumn(
                name: "PriceBeforeTax",
                table: "OrderDetails",
                newName: "PriceBeforePrivateTax");

            migrationBuilder.RenameColumn(
                name: "PriceAfterTax",
                table: "OrderDetails",
                newName: "PriceAfterPrivateTax");

            migrationBuilder.AddColumn<string>(
                name: "DefaultTax",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "OrderDetails",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderDetails",
                table: "OrderDetails",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_ProductId",
                table: "OrderDetails",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetail_Order",
                table: "OrderDetails",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetail_Product",
                table: "OrderDetails",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetail_Order",
                table: "OrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetail_Product",
                table: "OrderDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderDetails",
                table: "OrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_ProductId",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "DefaultTax",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "OrderDetails");

            migrationBuilder.RenameColumn(
                name: "TotalBeforeDefaultTax",
                table: "Orders",
                newName: "TotalBeforeTax");

            migrationBuilder.RenameColumn(
                name: "TotalAfterDefaultTax",
                table: "Orders",
                newName: "TotalAfterTax");

            migrationBuilder.RenameColumn(
                name: "PrivateTaxes",
                table: "OrderDetails",
                newName: "Taxes");

            migrationBuilder.RenameColumn(
                name: "PriceBeforePrivateTax",
                table: "OrderDetails",
                newName: "PriceBeforeTax");

            migrationBuilder.RenameColumn(
                name: "PriceAfterPrivateTax",
                table: "OrderDetails",
                newName: "PriceAfterTax");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderDetails",
                table: "OrderDetails",
                columns: new[] { "ProductId", "OrderId" });

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Orders_OrderId",
                table: "OrderDetails",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Product",
                table: "OrderDetails",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
