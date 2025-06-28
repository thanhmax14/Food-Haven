using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class migrationsv19 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("09cfdf19-e326-4ac1-bb38-c4aab67efb5d"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("144ccb64-8b5f-40fc-98a9-33008d732ca4"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("2c347a62-d4b5-4c71-b629-1470e0fc8c5d"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("8aca4aae-9744-43da-9a59-ae55fae77d6b"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("b47c3259-e5c4-4d9e-b19a-3e657cf56e53"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("f5c38cec-abbf-49b5-bade-c5dac2387781"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("2eaaa950-41f7-4473-95df-b7adcc804364"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("30a40a47-5d74-4b5a-a644-30f39cc5e240"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("53dbc6f8-bd09-4973-812b-8e1c8d0f40ab"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("a2f028eb-a033-453d-95d7-a83fc5bca4d1"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("c6080c76-e4d2-43ca-ae9d-b5b672343f0c"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("fd5f91e8-2358-4e52-88d8-9cadbf1c7176"));

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "FavoriteRecipes");

            migrationBuilder.AddColumn<float>(
                name: "CommissionPercent",
                table: "OrderDetails",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductTypeName",
                table: "OrderDetails",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "IngredientTag",
                columns: new[] { "ID", "CreatedDate", "IsActive", "ModifiedDate", "Name" },
                values: new object[,]
                {
                    { new Guid("1fee3db5-8ea6-4d41-8c89-240442e9ff55"), new DateTime(2025, 6, 28, 17, 28, 13, 547, DateTimeKind.Local).AddTicks(1678), true, null, "Pork" },
                    { new Guid("2304caed-08b0-402c-8138-3f8bb00979eb"), new DateTime(2025, 6, 28, 17, 28, 13, 547, DateTimeKind.Local).AddTicks(1684), true, null, "Vegetable" },
                    { new Guid("57363002-78ef-479d-8343-3b66c6edb77e"), new DateTime(2025, 6, 28, 17, 28, 13, 547, DateTimeKind.Local).AddTicks(1674), true, null, "Beef" },
                    { new Guid("8772558c-dc25-4322-a4a9-1cfc6564703b"), new DateTime(2025, 6, 28, 17, 28, 13, 547, DateTimeKind.Local).AddTicks(1667), true, null, "Fish" },
                    { new Guid("8788fedd-8dcc-4980-aa4c-19c1668cef7b"), new DateTime(2025, 6, 28, 17, 28, 13, 547, DateTimeKind.Local).AddTicks(1681), true, null, "Seafood" },
                    { new Guid("acf596ec-7124-4c19-aa8e-8dfdd48abb90"), new DateTime(2025, 6, 28, 17, 28, 13, 547, DateTimeKind.Local).AddTicks(1671), true, null, "Chicken" }
                });

            migrationBuilder.InsertData(
                table: "TypeOfDish",
                columns: new[] { "ID", "CreatedDate", "IsActive", "ModifiedDate", "Name" },
                values: new object[,]
                {
                    { new Guid("002d5df9-7fca-43cf-9bc7-148f7f1f1432"), new DateTime(2025, 6, 28, 17, 28, 13, 547, DateTimeKind.Local).AddTicks(1154), true, null, "Healthy Main Dishes" },
                    { new Guid("0c9e14fa-0980-4cec-815b-d8b7cef389a2"), new DateTime(2025, 6, 28, 17, 28, 13, 547, DateTimeKind.Local).AddTicks(1122), true, null, "Vegetarian Main Dishes" },
                    { new Guid("7ac1f909-b1ae-4a9d-bfc5-9dd2b6446c85"), new DateTime(2025, 6, 28, 17, 28, 13, 547, DateTimeKind.Local).AddTicks(1125), true, null, "Side Dishes" },
                    { new Guid("8f71054a-2f3b-43c4-9c85-2139be743efa"), new DateTime(2025, 6, 28, 17, 28, 13, 547, DateTimeKind.Local).AddTicks(1115), true, null, "Cooking for Two" },
                    { new Guid("c3ebbf01-079b-4918-ba31-05b396877838"), new DateTime(2025, 6, 28, 17, 28, 13, 547, DateTimeKind.Local).AddTicks(1119), true, null, "Main Dishes" },
                    { new Guid("d2e3b995-c870-483d-a076-4b9e88d2e439"), new DateTime(2025, 6, 28, 17, 28, 13, 547, DateTimeKind.Local).AddTicks(1111), true, null, "Quick and Easy Dinners for One" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("1fee3db5-8ea6-4d41-8c89-240442e9ff55"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("2304caed-08b0-402c-8138-3f8bb00979eb"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("57363002-78ef-479d-8343-3b66c6edb77e"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("8772558c-dc25-4322-a4a9-1cfc6564703b"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("8788fedd-8dcc-4980-aa4c-19c1668cef7b"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("acf596ec-7124-4c19-aa8e-8dfdd48abb90"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("002d5df9-7fca-43cf-9bc7-148f7f1f1432"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("0c9e14fa-0980-4cec-815b-d8b7cef389a2"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("7ac1f909-b1ae-4a9d-bfc5-9dd2b6446c85"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("8f71054a-2f3b-43c4-9c85-2139be743efa"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("c3ebbf01-079b-4918-ba31-05b396877838"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("d2e3b995-c870-483d-a076-4b9e88d2e439"));

            migrationBuilder.DropColumn(
                name: "CommissionPercent",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "ProductTypeName",
                table: "OrderDetails");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "FavoriteRecipes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "IngredientTag",
                columns: new[] { "ID", "CreatedDate", "IsActive", "ModifiedDate", "Name" },
                values: new object[,]
                {
                    { new Guid("09cfdf19-e326-4ac1-bb38-c4aab67efb5d"), new DateTime(2025, 6, 21, 14, 32, 41, 488, DateTimeKind.Local).AddTicks(2213), true, null, "Pork" },
                    { new Guid("144ccb64-8b5f-40fc-98a9-33008d732ca4"), new DateTime(2025, 6, 21, 14, 32, 41, 488, DateTimeKind.Local).AddTicks(2221), true, null, "Seafood" },
                    { new Guid("2c347a62-d4b5-4c71-b629-1470e0fc8c5d"), new DateTime(2025, 6, 21, 14, 32, 41, 488, DateTimeKind.Local).AddTicks(2199), true, null, "Fish" },
                    { new Guid("8aca4aae-9744-43da-9a59-ae55fae77d6b"), new DateTime(2025, 6, 21, 14, 32, 41, 488, DateTimeKind.Local).AddTicks(2209), true, null, "Beef" },
                    { new Guid("b47c3259-e5c4-4d9e-b19a-3e657cf56e53"), new DateTime(2025, 6, 21, 14, 32, 41, 488, DateTimeKind.Local).AddTicks(2206), true, null, "Chicken" },
                    { new Guid("f5c38cec-abbf-49b5-bade-c5dac2387781"), new DateTime(2025, 6, 21, 14, 32, 41, 488, DateTimeKind.Local).AddTicks(2225), true, null, "Vegetable" }
                });

            migrationBuilder.InsertData(
                table: "TypeOfDish",
                columns: new[] { "ID", "CreatedDate", "IsActive", "ModifiedDate", "Name" },
                values: new object[,]
                {
                    { new Guid("2eaaa950-41f7-4473-95df-b7adcc804364"), new DateTime(2025, 6, 21, 14, 32, 41, 488, DateTimeKind.Local).AddTicks(1602), true, null, "Vegetarian Main Dishes" },
                    { new Guid("30a40a47-5d74-4b5a-a644-30f39cc5e240"), new DateTime(2025, 6, 21, 14, 32, 41, 488, DateTimeKind.Local).AddTicks(1605), true, null, "Side Dishes" },
                    { new Guid("53dbc6f8-bd09-4973-812b-8e1c8d0f40ab"), new DateTime(2025, 6, 21, 14, 32, 41, 488, DateTimeKind.Local).AddTicks(1609), true, null, "Healthy Main Dishes" },
                    { new Guid("a2f028eb-a033-453d-95d7-a83fc5bca4d1"), new DateTime(2025, 6, 21, 14, 32, 41, 488, DateTimeKind.Local).AddTicks(1568), true, null, "Quick and Easy Dinners for One" },
                    { new Guid("c6080c76-e4d2-43ca-ae9d-b5b672343f0c"), new DateTime(2025, 6, 21, 14, 32, 41, 488, DateTimeKind.Local).AddTicks(1572), true, null, "Cooking for Two" },
                    { new Guid("fd5f91e8-2358-4e52-88d8-9cadbf1c7176"), new DateTime(2025, 6, 21, 14, 32, 41, 488, DateTimeKind.Local).AddTicks(1598), true, null, "Main Dishes" }
                });
        }
    }
}
