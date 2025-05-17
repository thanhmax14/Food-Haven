using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class migrationsv3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Products_ProductID",
                table: "OrderDetails");

            migrationBuilder.RenameColumn(
                name: "ProductID",
                table: "OrderDetails",
                newName: "ProductTypesID");

            migrationBuilder.RenameIndex(
                name: "IX_OrderDetails_ProductID",
                table: "OrderDetails",
                newName: "IX_OrderDetails_ProductTypesID");

            migrationBuilder.AddColumn<string>(
                name: "OrderTracking",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_ProductTypes_ProductTypesID",
                table: "OrderDetails",
                column: "ProductTypesID",
                principalTable: "ProductTypes",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_ProductTypes_ProductTypesID",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "OrderTracking",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "ProductTypesID",
                table: "OrderDetails",
                newName: "ProductID");

            migrationBuilder.RenameIndex(
                name: "IX_OrderDetails_ProductTypesID",
                table: "OrderDetails",
                newName: "IX_OrderDetails_ProductID");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Products_ProductID",
                table: "OrderDetails",
                column: "ProductID",
                principalTable: "Products",
                principalColumn: "ID");
        }
    }
}
