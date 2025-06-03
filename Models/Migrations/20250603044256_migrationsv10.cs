using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class migrationsv10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("0b522101-e4e2-4f15-8573-992a514be250"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("608f5e0f-3388-47b3-a976-b16410c65055"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("679d010e-8802-4c5f-ab22-3cba12059e39"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("7b87bca3-22cf-4100-8aa7-105dd9403a9c"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("d49ac81e-4b09-495e-b1fa-a49f00215479"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("f8b8937a-56a2-4577-a5b2-020b162d46e7"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("1bc7452a-5e93-4b52-b6f7-5c0cfeb64bb6"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("1dafc904-c95d-49ac-97ba-9d6ebe51118b"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("46605b48-6232-4e7b-9a72-17e6dd6ef933"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("8678ca3a-91e1-45cb-bdca-b50e56c774a6"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("930e5baa-344f-4700-8c08-0a930295f40b"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("b7c0eee7-48cb-4b99-946e-6321f1ca9e06"));

            migrationBuilder.AddColumn<bool>(
                name: "IsGlobal",
                table: "Vouchers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "StoreID",
                table: "Vouchers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.InsertData(
                table: "IngredientTag",
                columns: new[] { "ID", "CreatedDate", "IsActive", "ModifiedDate", "Name" },
                values: new object[,]
                {
                    { new Guid("05c95f77-f2a1-4cfc-9250-1996386aede7"), new DateTime(2025, 6, 3, 11, 42, 55, 24, DateTimeKind.Local).AddTicks(8063), true, null, "Pork" },
                    { new Guid("1a0c4a7e-372a-48c0-baaf-d2414c03b659"), new DateTime(2025, 6, 3, 11, 42, 55, 24, DateTimeKind.Local).AddTicks(8060), true, null, "Beef" },
                    { new Guid("228e7e73-6802-4e5e-a188-e2c00e5776d6"), new DateTime(2025, 6, 3, 11, 42, 55, 24, DateTimeKind.Local).AddTicks(8054), true, null, "Fish" },
                    { new Guid("5ff5a9dd-9664-49b3-b60d-f9fedc9b93a4"), new DateTime(2025, 6, 3, 11, 42, 55, 24, DateTimeKind.Local).AddTicks(8057), true, null, "Chicken" },
                    { new Guid("a73f2b3f-a271-4b45-b3e4-ebfd1264abfc"), new DateTime(2025, 6, 3, 11, 42, 55, 24, DateTimeKind.Local).AddTicks(8068), true, null, "Vegetable" },
                    { new Guid("fc0b8060-6945-4ced-92fb-130e446fc24a"), new DateTime(2025, 6, 3, 11, 42, 55, 24, DateTimeKind.Local).AddTicks(8065), true, null, "Seafood" }
                });

            migrationBuilder.InsertData(
                table: "TypeOfDish",
                columns: new[] { "ID", "CreatedDate", "IsActive", "ModifiedDate", "Name" },
                values: new object[,]
                {
                    { new Guid("6e4abc7f-4141-4be2-8da4-ecbcf87425e6"), new DateTime(2025, 6, 3, 11, 42, 55, 24, DateTimeKind.Local).AddTicks(7603), true, null, "Main Dishes" },
                    { new Guid("78f3cc41-e5d7-4f42-9270-2e91f38c08a7"), new DateTime(2025, 6, 3, 11, 42, 55, 24, DateTimeKind.Local).AddTicks(7600), true, null, "Cooking for Two" },
                    { new Guid("90f12565-b92b-4ab3-80a8-15e08083b1ec"), new DateTime(2025, 6, 3, 11, 42, 55, 24, DateTimeKind.Local).AddTicks(7648), true, null, "Healthy Main Dishes" },
                    { new Guid("9c9515c9-1f6d-470f-b23f-6f78877e4ba4"), new DateTime(2025, 6, 3, 11, 42, 55, 24, DateTimeKind.Local).AddTicks(7597), true, null, "Quick and Easy Dinners for One" },
                    { new Guid("c8522c52-7e89-4de1-8fe5-26ba3f2cb6f9"), new DateTime(2025, 6, 3, 11, 42, 55, 24, DateTimeKind.Local).AddTicks(7608), true, null, "Side Dishes" },
                    { new Guid("f01f726f-161e-4835-aeef-46c3eefe9335"), new DateTime(2025, 6, 3, 11, 42, 55, 24, DateTimeKind.Local).AddTicks(7605), true, null, "Vegetarian Main Dishes" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Vouchers_StoreID",
                table: "Vouchers",
                column: "StoreID");

            migrationBuilder.AddForeignKey(
                name: "FK_Vouchers_StoreDetails_StoreID",
                table: "Vouchers",
                column: "StoreID",
                principalTable: "StoreDetails",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vouchers_StoreDetails_StoreID",
                table: "Vouchers");

            migrationBuilder.DropIndex(
                name: "IX_Vouchers_StoreID",
                table: "Vouchers");

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("05c95f77-f2a1-4cfc-9250-1996386aede7"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("1a0c4a7e-372a-48c0-baaf-d2414c03b659"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("228e7e73-6802-4e5e-a188-e2c00e5776d6"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("5ff5a9dd-9664-49b3-b60d-f9fedc9b93a4"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("a73f2b3f-a271-4b45-b3e4-ebfd1264abfc"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("fc0b8060-6945-4ced-92fb-130e446fc24a"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("6e4abc7f-4141-4be2-8da4-ecbcf87425e6"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("78f3cc41-e5d7-4f42-9270-2e91f38c08a7"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("90f12565-b92b-4ab3-80a8-15e08083b1ec"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("9c9515c9-1f6d-470f-b23f-6f78877e4ba4"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("c8522c52-7e89-4de1-8fe5-26ba3f2cb6f9"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("f01f726f-161e-4835-aeef-46c3eefe9335"));

            migrationBuilder.DropColumn(
                name: "IsGlobal",
                table: "Vouchers");

            migrationBuilder.DropColumn(
                name: "StoreID",
                table: "Vouchers");

            migrationBuilder.InsertData(
                table: "IngredientTag",
                columns: new[] { "ID", "CreatedDate", "IsActive", "ModifiedDate", "Name" },
                values: new object[,]
                {
                    { new Guid("0b522101-e4e2-4f15-8573-992a514be250"), new DateTime(2025, 6, 3, 11, 31, 2, 247, DateTimeKind.Local).AddTicks(8134), true, null, "Beef" },
                    { new Guid("608f5e0f-3388-47b3-a976-b16410c65055"), new DateTime(2025, 6, 3, 11, 31, 2, 247, DateTimeKind.Local).AddTicks(8099), true, null, "Fish" },
                    { new Guid("679d010e-8802-4c5f-ab22-3cba12059e39"), new DateTime(2025, 6, 3, 11, 31, 2, 247, DateTimeKind.Local).AddTicks(8139), true, null, "Pork" },
                    { new Guid("7b87bca3-22cf-4100-8aa7-105dd9403a9c"), new DateTime(2025, 6, 3, 11, 31, 2, 247, DateTimeKind.Local).AddTicks(8128), true, null, "Chicken" },
                    { new Guid("d49ac81e-4b09-495e-b1fa-a49f00215479"), new DateTime(2025, 6, 3, 11, 31, 2, 247, DateTimeKind.Local).AddTicks(8149), true, null, "Vegetable" },
                    { new Guid("f8b8937a-56a2-4577-a5b2-020b162d46e7"), new DateTime(2025, 6, 3, 11, 31, 2, 247, DateTimeKind.Local).AddTicks(8144), true, null, "Seafood" }
                });

            migrationBuilder.InsertData(
                table: "TypeOfDish",
                columns: new[] { "ID", "CreatedDate", "IsActive", "ModifiedDate", "Name" },
                values: new object[,]
                {
                    { new Guid("1bc7452a-5e93-4b52-b6f7-5c0cfeb64bb6"), new DateTime(2025, 6, 3, 11, 31, 2, 247, DateTimeKind.Local).AddTicks(7314), true, null, "Vegetarian Main Dishes" },
                    { new Guid("1dafc904-c95d-49ac-97ba-9d6ebe51118b"), new DateTime(2025, 6, 3, 11, 31, 2, 247, DateTimeKind.Local).AddTicks(7319), true, null, "Side Dishes" },
                    { new Guid("46605b48-6232-4e7b-9a72-17e6dd6ef933"), new DateTime(2025, 6, 3, 11, 31, 2, 247, DateTimeKind.Local).AddTicks(7308), true, null, "Main Dishes" },
                    { new Guid("8678ca3a-91e1-45cb-bdca-b50e56c774a6"), new DateTime(2025, 6, 3, 11, 31, 2, 247, DateTimeKind.Local).AddTicks(7302), true, null, "Cooking for Two" },
                    { new Guid("930e5baa-344f-4700-8c08-0a930295f40b"), new DateTime(2025, 6, 3, 11, 31, 2, 247, DateTimeKind.Local).AddTicks(7296), true, null, "Quick and Easy Dinners for One" },
                    { new Guid("b7c0eee7-48cb-4b99-946e-6321f1ca9e06"), new DateTime(2025, 6, 3, 11, 31, 2, 247, DateTimeKind.Local).AddTicks(7323), true, null, "Healthy Main Dishes" }
                });
        }
    }
}
