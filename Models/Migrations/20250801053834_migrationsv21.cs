using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class migrationsv21 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("1979bb10-01a4-4f4c-aa1e-a12eee3f5fcd"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("2836dccd-9b07-46dc-b23f-caeb2674fc31"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("37a7812e-ee96-4afa-89b6-ccd7d9216be3"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("831d4e64-e08d-49bd-972e-3ecccba519ee"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("8ddd0532-826a-40e9-94d7-d84dba4eccbf"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("eaf4d049-96f7-4e3c-8f67-492cf3bd4f02"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("35d8f123-38af-4840-893e-fb676ea776e6"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("4257ddbe-dcbd-4207-a666-402b05553197"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("48d6a26d-a523-4303-ba56-bfca4670cb4a"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("838c0191-e5d1-4e4f-be9f-b622404e512f"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("f084cea9-aa13-4c7b-8235-a7c741730222"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("f5a2371c-da6b-4fcd-9f4d-c342946df28d"));

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "ExpertRecipe",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "NER",
                table: "ExpertRecipe",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Link",
                table: "ExpertRecipe",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Ingredients",
                table: "ExpertRecipe",
                type: "nvarchar(1500)",
                maxLength: 1500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Directions",
                table: "ExpertRecipe",
                type: "nvarchar(2500)",
                maxLength: 2500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "ExpertRecipe",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "ExpertRecipe",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "ExpertRecipe",
                type: "datetime2",
                nullable: true);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "ExpertRecipe");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "ExpertRecipe");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "ExpertRecipe");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "ExpertRecipe",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "NER",
                table: "ExpertRecipe",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "Link",
                table: "ExpertRecipe",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Ingredients",
                table: "ExpertRecipe",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1500)",
                oldMaxLength: 1500);

            migrationBuilder.AlterColumn<string>(
                name: "Directions",
                table: "ExpertRecipe",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(2500)",
                oldMaxLength: 2500);

            migrationBuilder.InsertData(
                table: "IngredientTag",
                columns: new[] { "ID", "CreatedDate", "IsActive", "ModifiedDate", "Name" },
                values: new object[,]
                {
                    { new Guid("1979bb10-01a4-4f4c-aa1e-a12eee3f5fcd"), new DateTime(2025, 7, 30, 0, 14, 45, 883, DateTimeKind.Local).AddTicks(7721), true, null, "Beef" },
                    { new Guid("2836dccd-9b07-46dc-b23f-caeb2674fc31"), new DateTime(2025, 7, 30, 0, 14, 45, 883, DateTimeKind.Local).AddTicks(7733), true, null, "Vegetable" },
                    { new Guid("37a7812e-ee96-4afa-89b6-ccd7d9216be3"), new DateTime(2025, 7, 30, 0, 14, 45, 883, DateTimeKind.Local).AddTicks(7729), true, null, "Seafood" },
                    { new Guid("831d4e64-e08d-49bd-972e-3ecccba519ee"), new DateTime(2025, 7, 30, 0, 14, 45, 883, DateTimeKind.Local).AddTicks(7711), true, null, "Chicken" },
                    { new Guid("8ddd0532-826a-40e9-94d7-d84dba4eccbf"), new DateTime(2025, 7, 30, 0, 14, 45, 883, DateTimeKind.Local).AddTicks(7725), true, null, "Pork" },
                    { new Guid("eaf4d049-96f7-4e3c-8f67-492cf3bd4f02"), new DateTime(2025, 7, 30, 0, 14, 45, 883, DateTimeKind.Local).AddTicks(7706), true, null, "Fish" }
                });

            migrationBuilder.InsertData(
                table: "TypeOfDish",
                columns: new[] { "ID", "CreatedDate", "IsActive", "ModifiedDate", "Name" },
                values: new object[,]
                {
                    { new Guid("35d8f123-38af-4840-893e-fb676ea776e6"), new DateTime(2025, 7, 30, 0, 14, 45, 883, DateTimeKind.Local).AddTicks(7037), true, null, "Side Dishes" },
                    { new Guid("4257ddbe-dcbd-4207-a666-402b05553197"), new DateTime(2025, 7, 30, 0, 14, 45, 883, DateTimeKind.Local).AddTicks(7019), true, null, "Quick and Easy Dinners for One" },
                    { new Guid("48d6a26d-a523-4303-ba56-bfca4670cb4a"), new DateTime(2025, 7, 30, 0, 14, 45, 883, DateTimeKind.Local).AddTicks(7033), true, null, "Vegetarian Main Dishes" },
                    { new Guid("838c0191-e5d1-4e4f-be9f-b622404e512f"), new DateTime(2025, 7, 30, 0, 14, 45, 883, DateTimeKind.Local).AddTicks(7042), true, null, "Healthy Main Dishes" },
                    { new Guid("f084cea9-aa13-4c7b-8235-a7c741730222"), new DateTime(2025, 7, 30, 0, 14, 45, 883, DateTimeKind.Local).AddTicks(7029), true, null, "Main Dishes" },
                    { new Guid("f5a2371c-da6b-4fcd-9f4d-c342946df28d"), new DateTime(2025, 7, 30, 0, 14, 45, 883, DateTimeKind.Local).AddTicks(7024), true, null, "Cooking for Two" }
                });
        }
    }
}
