using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class migrationsv17 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("07551154-2960-42b6-84b9-4d2ad4f6e009"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("19141ee9-68ac-4ff1-82fa-cc3b47b9d03b"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("97b15834-2126-4d3d-b4b3-cb3ea4203b2c"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("a35eb1d4-c720-4669-b1b4-9c75828085aa"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("bff00ebe-9453-4936-b334-f4712a03e580"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("cb1c72d0-dc75-4088-aa80-6096835b9244"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("662db111-66d0-4469-82a4-4591da0d1286"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("9a78507d-0699-4993-a586-b5aa98708190"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("a4ca95bc-9b32-47f0-9f2c-2666c0f7a575"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("a8c1adf7-213b-4ed0-91a0-45680c8033c2"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("adfbab0d-af4d-4903-91ce-90a5ca01ae7a"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("aeb56d6d-d7fe-495c-be3c-c29531182523"));

            migrationBuilder.InsertData(
                table: "IngredientTag",
                columns: new[] { "ID", "CreatedDate", "IsActive", "ModifiedDate", "Name" },
                values: new object[,]
                {
                    { new Guid("21e1b697-887b-498b-b0d0-98f4a8f9d942"), new DateTime(2025, 6, 18, 17, 24, 11, 24, DateTimeKind.Local).AddTicks(987), true, null, "Pork" },
                    { new Guid("2d33a66b-1a05-451a-acc8-ef821a56a4b7"), new DateTime(2025, 6, 18, 17, 24, 11, 24, DateTimeKind.Local).AddTicks(982), true, null, "Chicken" },
                    { new Guid("5be61e5b-6837-46a2-b4ca-8422d81e5653"), new DateTime(2025, 6, 18, 17, 24, 11, 24, DateTimeKind.Local).AddTicks(989), true, null, "Seafood" },
                    { new Guid("893dded5-05ac-4c94-932d-7f1e2acde4e6"), new DateTime(2025, 6, 18, 17, 24, 11, 24, DateTimeKind.Local).AddTicks(979), true, null, "Fish" },
                    { new Guid("acdee6d2-2a4f-4b35-a0cc-30b0733034fd"), new DateTime(2025, 6, 18, 17, 24, 11, 24, DateTimeKind.Local).AddTicks(984), true, null, "Beef" },
                    { new Guid("b9fb2da7-c0a2-4a5d-a0fd-7dc7064aa2a8"), new DateTime(2025, 6, 18, 17, 24, 11, 24, DateTimeKind.Local).AddTicks(996), true, null, "Vegetable" }
                });

            migrationBuilder.InsertData(
                table: "TypeOfDish",
                columns: new[] { "ID", "CreatedDate", "IsActive", "ModifiedDate", "Name" },
                values: new object[,]
                {
                    { new Guid("3cbc1377-b7ec-4369-af83-cc3e55f29fb2"), new DateTime(2025, 6, 18, 17, 24, 11, 24, DateTimeKind.Local).AddTicks(593), true, null, "Healthy Main Dishes" },
                    { new Guid("72ff1704-8d63-4d58-929b-2769e10f3be6"), new DateTime(2025, 6, 18, 17, 24, 11, 24, DateTimeKind.Local).AddTicks(570), true, null, "Main Dishes" },
                    { new Guid("8522856a-fc06-4d88-be80-e7664de23a73"), new DateTime(2025, 6, 18, 17, 24, 11, 24, DateTimeKind.Local).AddTicks(590), true, null, "Side Dishes" },
                    { new Guid("b0f19f66-55a6-4288-945b-6f2d3e203e04"), new DateTime(2025, 6, 18, 17, 24, 11, 24, DateTimeKind.Local).AddTicks(588), true, null, "Vegetarian Main Dishes" },
                    { new Guid("c335e392-4751-4fb1-a764-8915574e8972"), new DateTime(2025, 6, 18, 17, 24, 11, 24, DateTimeKind.Local).AddTicks(567), true, null, "Cooking for Two" },
                    { new Guid("d4bf96bf-f907-43cf-9c0d-2974f222706e"), new DateTime(2025, 6, 18, 17, 24, 11, 24, DateTimeKind.Local).AddTicks(565), true, null, "Quick and Easy Dinners for One" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("21e1b697-887b-498b-b0d0-98f4a8f9d942"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("2d33a66b-1a05-451a-acc8-ef821a56a4b7"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("5be61e5b-6837-46a2-b4ca-8422d81e5653"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("893dded5-05ac-4c94-932d-7f1e2acde4e6"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("acdee6d2-2a4f-4b35-a0cc-30b0733034fd"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("b9fb2da7-c0a2-4a5d-a0fd-7dc7064aa2a8"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("3cbc1377-b7ec-4369-af83-cc3e55f29fb2"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("72ff1704-8d63-4d58-929b-2769e10f3be6"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("8522856a-fc06-4d88-be80-e7664de23a73"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("b0f19f66-55a6-4288-945b-6f2d3e203e04"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("c335e392-4751-4fb1-a764-8915574e8972"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("d4bf96bf-f907-43cf-9c0d-2974f222706e"));

            migrationBuilder.InsertData(
                table: "IngredientTag",
                columns: new[] { "ID", "CreatedDate", "IsActive", "ModifiedDate", "Name" },
                values: new object[,]
                {
                    { new Guid("07551154-2960-42b6-84b9-4d2ad4f6e009"), new DateTime(2025, 6, 18, 17, 22, 16, 806, DateTimeKind.Local).AddTicks(2373), true, null, "Beef" },
                    { new Guid("19141ee9-68ac-4ff1-82fa-cc3b47b9d03b"), new DateTime(2025, 6, 18, 17, 22, 16, 806, DateTimeKind.Local).AddTicks(2378), true, null, "Pork" },
                    { new Guid("97b15834-2126-4d3d-b4b3-cb3ea4203b2c"), new DateTime(2025, 6, 18, 17, 22, 16, 806, DateTimeKind.Local).AddTicks(2338), true, null, "Fish" },
                    { new Guid("a35eb1d4-c720-4669-b1b4-9c75828085aa"), new DateTime(2025, 6, 18, 17, 22, 16, 806, DateTimeKind.Local).AddTicks(2383), true, null, "Seafood" },
                    { new Guid("bff00ebe-9453-4936-b334-f4712a03e580"), new DateTime(2025, 6, 18, 17, 22, 16, 806, DateTimeKind.Local).AddTicks(2387), true, null, "Vegetable" },
                    { new Guid("cb1c72d0-dc75-4088-aa80-6096835b9244"), new DateTime(2025, 6, 18, 17, 22, 16, 806, DateTimeKind.Local).AddTicks(2345), true, null, "Chicken" }
                });

            migrationBuilder.InsertData(
                table: "TypeOfDish",
                columns: new[] { "ID", "CreatedDate", "IsActive", "ModifiedDate", "Name" },
                values: new object[,]
                {
                    { new Guid("662db111-66d0-4469-82a4-4591da0d1286"), new DateTime(2025, 6, 18, 17, 22, 16, 806, DateTimeKind.Local).AddTicks(1631), true, null, "Side Dishes" },
                    { new Guid("9a78507d-0699-4993-a586-b5aa98708190"), new DateTime(2025, 6, 18, 17, 22, 16, 806, DateTimeKind.Local).AddTicks(1627), true, null, "Vegetarian Main Dishes" },
                    { new Guid("a4ca95bc-9b32-47f0-9f2c-2666c0f7a575"), new DateTime(2025, 6, 18, 17, 22, 16, 806, DateTimeKind.Local).AddTicks(1614), true, null, "Quick and Easy Dinners for One" },
                    { new Guid("a8c1adf7-213b-4ed0-91a0-45680c8033c2"), new DateTime(2025, 6, 18, 17, 22, 16, 806, DateTimeKind.Local).AddTicks(1635), true, null, "Healthy Main Dishes" },
                    { new Guid("adfbab0d-af4d-4903-91ce-90a5ca01ae7a"), new DateTime(2025, 6, 18, 17, 22, 16, 806, DateTimeKind.Local).AddTicks(1619), true, null, "Cooking for Two" },
                    { new Guid("aeb56d6d-d7fe-495c-be3c-c29531182523"), new DateTime(2025, 6, 18, 17, 22, 16, 806, DateTimeKind.Local).AddTicks(1623), true, null, "Main Dishes" }
                });
        }
    }
}
