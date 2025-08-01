using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class migrationsv23 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.InsertData(
                table: "IngredientTag",
                columns: new[] { "ID", "CreatedDate", "IsActive", "ModifiedDate", "Name" },
                values: new object[,]
                {
                    { new Guid("413744b5-a2a2-4e41-8ce6-1364011bc70f"), new DateTime(2025, 8, 1, 12, 41, 40, 478, DateTimeKind.Local).AddTicks(4093), true, null, "Chicken" },
                    { new Guid("5a930780-4e75-4c3f-ae4f-9362c220289c"), new DateTime(2025, 8, 1, 12, 41, 40, 478, DateTimeKind.Local).AddTicks(4107), true, null, "Seafood" },
                    { new Guid("72d389e8-ca3c-41a6-acae-1c1bcec452d2"), new DateTime(2025, 8, 1, 12, 41, 40, 478, DateTimeKind.Local).AddTicks(4091), true, null, "Fish" },
                    { new Guid("b0ec2054-02f8-4639-a137-4afe58fe3e15"), new DateTime(2025, 8, 1, 12, 41, 40, 478, DateTimeKind.Local).AddTicks(4104), true, null, "Pork" },
                    { new Guid("eadd8e21-ca2b-4be6-ad80-b0924156d4b7"), new DateTime(2025, 8, 1, 12, 41, 40, 478, DateTimeKind.Local).AddTicks(4109), true, null, "Vegetable" },
                    { new Guid("f70dbb32-5282-4bcd-ba40-1645a5ee402d"), new DateTime(2025, 8, 1, 12, 41, 40, 478, DateTimeKind.Local).AddTicks(4101), true, null, "Beef" }
                });

            migrationBuilder.InsertData(
                table: "TypeOfDish",
                columns: new[] { "ID", "CreatedDate", "IsActive", "ModifiedDate", "Name" },
                values: new object[,]
                {
                    { new Guid("4e5a04ae-1e18-4324-af89-9e562a047cb7"), new DateTime(2025, 8, 1, 12, 41, 40, 478, DateTimeKind.Local).AddTicks(3629), true, null, "Healthy Main Dishes" },
                    { new Guid("564a01bb-e694-4022-945c-2d2798371d82"), new DateTime(2025, 8, 1, 12, 41, 40, 478, DateTimeKind.Local).AddTicks(3620), true, null, "Main Dishes" },
                    { new Guid("63a7a0b8-9b4d-42d9-87e4-a1bcfa9fe62a"), new DateTime(2025, 8, 1, 12, 41, 40, 478, DateTimeKind.Local).AddTicks(3623), true, null, "Vegetarian Main Dishes" },
                    { new Guid("b29beb6c-bea7-4801-b174-5cedc60c40ad"), new DateTime(2025, 8, 1, 12, 41, 40, 478, DateTimeKind.Local).AddTicks(3614), true, null, "Quick and Easy Dinners for One" },
                    { new Guid("da1955b6-892e-4642-ae7d-cfe1a6a7aa0d"), new DateTime(2025, 8, 1, 12, 41, 40, 478, DateTimeKind.Local).AddTicks(3626), true, null, "Side Dishes" },
                    { new Guid("df8ceab4-123c-4a00-aedf-a08493448cce"), new DateTime(2025, 8, 1, 12, 41, 40, 478, DateTimeKind.Local).AddTicks(3617), true, null, "Cooking for Two" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("413744b5-a2a2-4e41-8ce6-1364011bc70f"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("5a930780-4e75-4c3f-ae4f-9362c220289c"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("72d389e8-ca3c-41a6-acae-1c1bcec452d2"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("b0ec2054-02f8-4639-a137-4afe58fe3e15"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("eadd8e21-ca2b-4be6-ad80-b0924156d4b7"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("f70dbb32-5282-4bcd-ba40-1645a5ee402d"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("4e5a04ae-1e18-4324-af89-9e562a047cb7"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("564a01bb-e694-4022-945c-2d2798371d82"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("63a7a0b8-9b4d-42d9-87e4-a1bcfa9fe62a"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("b29beb6c-bea7-4801-b174-5cedc60c40ad"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("da1955b6-892e-4642-ae7d-cfe1a6a7aa0d"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("df8ceab4-123c-4a00-aedf-a08493448cce"));

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
    }
}
