using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class migrationsv22 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("02ca9ed0-f05e-4b71-814e-c6c21b0b37c1"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("195143e7-094d-48d0-af78-9bf4a3e86682"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("334d77b3-0e9c-447c-ac5c-c01ca6fd4b5f"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("3933f320-76f2-443f-8f70-76d7b1b7998e"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("69943e5f-7372-489a-8a12-68c8f94c8d2c"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("b7c3a879-fce4-4d6d-8721-5970fe49cc15"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("202faf58-0019-4a97-b3bb-da799cf8d479"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("45921b98-c5d2-45ca-a06d-597f5b4d80d7"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("5c282f80-5bc5-410f-9b59-647e8cbcba77"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("613158f9-e378-4dc9-bca4-df64b9ccec4c"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("b3295ef6-5ffc-431b-86b9-ed1cc80db4b8"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("d4b1dcdf-47dd-4ce2-9448-9ea01869b2af"));

            migrationBuilder.AlterColumn<string>(
                name: "MatchedIngredients",
                table: "RecipeViewHistory",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Source",
                table: "ExpertRecipe",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.InsertData(
                table: "IngredientTag",
                columns: new[] { "ID", "CreatedDate", "IsActive", "ModifiedDate", "Name" },
                values: new object[,]
                {
                    { new Guid("78f174af-7174-4f4e-9c0e-b699a3b14a4d"), new DateTime(2025, 8, 1, 12, 41, 23, 872, DateTimeKind.Local).AddTicks(7388), true, null, "Beef" },
                    { new Guid("7eb9b7d9-6e2b-4de1-aff2-23b781f7a729"), new DateTime(2025, 8, 1, 12, 41, 23, 872, DateTimeKind.Local).AddTicks(7390), true, null, "Pork" },
                    { new Guid("82dc6f85-92d1-4b11-9c98-c372b327822a"), new DateTime(2025, 8, 1, 12, 41, 23, 872, DateTimeKind.Local).AddTicks(7395), true, null, "Vegetable" },
                    { new Guid("bb5747fe-66b1-441c-8b4a-04cb2d5dcc70"), new DateTime(2025, 8, 1, 12, 41, 23, 872, DateTimeKind.Local).AddTicks(7383), true, null, "Fish" },
                    { new Guid("c3136322-8baa-4bbd-be1f-9c2cd7efe9e8"), new DateTime(2025, 8, 1, 12, 41, 23, 872, DateTimeKind.Local).AddTicks(7392), true, null, "Seafood" },
                    { new Guid("c4f0f04b-3c26-49b0-8145-d737a7790d8f"), new DateTime(2025, 8, 1, 12, 41, 23, 872, DateTimeKind.Local).AddTicks(7385), true, null, "Chicken" }
                });

            migrationBuilder.InsertData(
                table: "TypeOfDish",
                columns: new[] { "ID", "CreatedDate", "IsActive", "ModifiedDate", "Name" },
                values: new object[,]
                {
                    { new Guid("0c8e00b3-a524-474a-9c6d-60527f3e7d34"), new DateTime(2025, 8, 1, 12, 41, 23, 872, DateTimeKind.Local).AddTicks(6983), true, null, "Vegetarian Main Dishes" },
                    { new Guid("0d8416ec-4dea-43b9-ba1b-4bc35c04ce60"), new DateTime(2025, 8, 1, 12, 41, 23, 872, DateTimeKind.Local).AddTicks(6985), true, null, "Side Dishes" },
                    { new Guid("46774101-cd8e-4a74-9856-b6a15d5d31d5"), new DateTime(2025, 8, 1, 12, 41, 23, 872, DateTimeKind.Local).AddTicks(6977), true, null, "Cooking for Two" },
                    { new Guid("631ed0ba-d9b8-4be5-9004-b8eaa95a92d9"), new DateTime(2025, 8, 1, 12, 41, 23, 872, DateTimeKind.Local).AddTicks(6974), true, null, "Quick and Easy Dinners for One" },
                    { new Guid("cbed1d1e-b5b6-4a67-83b5-5b31eafd8850"), new DateTime(2025, 8, 1, 12, 41, 23, 872, DateTimeKind.Local).AddTicks(7004), true, null, "Healthy Main Dishes" },
                    { new Guid("df885e97-080c-4b98-95ad-928f7ba1f70b"), new DateTime(2025, 8, 1, 12, 41, 23, 872, DateTimeKind.Local).AddTicks(6981), true, null, "Main Dishes" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("78f174af-7174-4f4e-9c0e-b699a3b14a4d"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("7eb9b7d9-6e2b-4de1-aff2-23b781f7a729"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("82dc6f85-92d1-4b11-9c98-c372b327822a"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("bb5747fe-66b1-441c-8b4a-04cb2d5dcc70"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("c3136322-8baa-4bbd-be1f-9c2cd7efe9e8"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("c4f0f04b-3c26-49b0-8145-d737a7790d8f"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("0c8e00b3-a524-474a-9c6d-60527f3e7d34"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("0d8416ec-4dea-43b9-ba1b-4bc35c04ce60"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("46774101-cd8e-4a74-9856-b6a15d5d31d5"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("631ed0ba-d9b8-4be5-9004-b8eaa95a92d9"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("cbed1d1e-b5b6-4a67-83b5-5b31eafd8850"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("df885e97-080c-4b98-95ad-928f7ba1f70b"));

            migrationBuilder.AlterColumn<string>(
                name: "MatchedIngredients",
                table: "RecipeViewHistory",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "Source",
                table: "ExpertRecipe",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.InsertData(
                table: "IngredientTag",
                columns: new[] { "ID", "CreatedDate", "IsActive", "ModifiedDate", "Name" },
                values: new object[,]
                {
                    { new Guid("02ca9ed0-f05e-4b71-814e-c6c21b0b37c1"), new DateTime(2025, 8, 1, 12, 38, 32, 876, DateTimeKind.Local).AddTicks(944), true, null, "Vegetable" },
                    { new Guid("195143e7-094d-48d0-af78-9bf4a3e86682"), new DateTime(2025, 8, 1, 12, 38, 32, 876, DateTimeKind.Local).AddTicks(938), true, null, "Pork" },
                    { new Guid("334d77b3-0e9c-447c-ac5c-c01ca6fd4b5f"), new DateTime(2025, 8, 1, 12, 38, 32, 876, DateTimeKind.Local).AddTicks(931), true, null, "Chicken" },
                    { new Guid("3933f320-76f2-443f-8f70-76d7b1b7998e"), new DateTime(2025, 8, 1, 12, 38, 32, 876, DateTimeKind.Local).AddTicks(927), true, null, "Fish" },
                    { new Guid("69943e5f-7372-489a-8a12-68c8f94c8d2c"), new DateTime(2025, 8, 1, 12, 38, 32, 876, DateTimeKind.Local).AddTicks(941), true, null, "Seafood" },
                    { new Guid("b7c3a879-fce4-4d6d-8721-5970fe49cc15"), new DateTime(2025, 8, 1, 12, 38, 32, 876, DateTimeKind.Local).AddTicks(934), true, null, "Beef" }
                });

            migrationBuilder.InsertData(
                table: "TypeOfDish",
                columns: new[] { "ID", "CreatedDate", "IsActive", "ModifiedDate", "Name" },
                values: new object[,]
                {
                    { new Guid("202faf58-0019-4a97-b3bb-da799cf8d479"), new DateTime(2025, 8, 1, 12, 38, 32, 876, DateTimeKind.Local).AddTicks(468), true, null, "Healthy Main Dishes" },
                    { new Guid("45921b98-c5d2-45ca-a06d-597f5b4d80d7"), new DateTime(2025, 8, 1, 12, 38, 32, 876, DateTimeKind.Local).AddTicks(437), true, null, "Cooking for Two" },
                    { new Guid("5c282f80-5bc5-410f-9b59-647e8cbcba77"), new DateTime(2025, 8, 1, 12, 38, 32, 876, DateTimeKind.Local).AddTicks(440), true, null, "Main Dishes" },
                    { new Guid("613158f9-e378-4dc9-bca4-df64b9ccec4c"), new DateTime(2025, 8, 1, 12, 38, 32, 876, DateTimeKind.Local).AddTicks(443), true, null, "Vegetarian Main Dishes" },
                    { new Guid("b3295ef6-5ffc-431b-86b9-ed1cc80db4b8"), new DateTime(2025, 8, 1, 12, 38, 32, 876, DateTimeKind.Local).AddTicks(433), true, null, "Quick and Easy Dinners for One" },
                    { new Guid("d4b1dcdf-47dd-4ce2-9448-9ea01869b2af"), new DateTime(2025, 8, 1, 12, 38, 32, 876, DateTimeKind.Local).AddTicks(465), true, null, "Side Dishes" }
                });
        }
    }
}
