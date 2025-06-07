using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class migrationsv12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("02aecfaa-e550-416a-ac68-b79105abb772"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("1523546b-43ca-470d-bdf3-064b68cf8327"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("174c4ccf-82ba-43e4-bd4a-7ee12491542e"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("772cfe90-4054-4ee5-b6ad-a97998b07b05"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("78a548ae-09ef-43eb-a4ee-5c87bec4dacc"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("f3708c7a-9ec0-4440-a576-acf378f8f703"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("14f70984-edee-464c-a1f5-514d56615422"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("308788ef-86c7-449c-a6f0-e6c4f58df002"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("441ee5cb-1b9e-4308-a3a0-4436f6172705"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("6af478fe-bb61-43de-bc2a-98ad069ed6f0"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("81c8b8b8-b1b5-4fdb-afd3-f22a9e97917b"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("ea20ddfc-75c1-4d59-8482-987c75a3c415"));

            migrationBuilder.DropColumn(
                name: "Scope",
                table: "Vouchers");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Vouchers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MaxDiscountAmount",
                table: "Vouchers",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsagePerUser",
                table: "Vouchers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectNote",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectNote",
                table: "StoreDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectNote",
                table: "Recipes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "status",
                table: "Recipes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "view",
                table: "Recipes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "RejectNote",
                table: "Complaints",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectNote",
                table: "BalanceChanges",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.InsertData(
                table: "IngredientTag",
                columns: new[] { "ID", "CreatedDate", "IsActive", "ModifiedDate", "Name" },
                values: new object[,]
                {
                    { new Guid("04e1d6cf-5c29-452d-803f-d62cb7ba5570"), new DateTime(2025, 6, 7, 19, 34, 19, 104, DateTimeKind.Local).AddTicks(58), true, null, "Fish" },
                    { new Guid("0c3df224-5b6b-4e9b-85fd-a2a8c3d258bb"), new DateTime(2025, 6, 7, 19, 34, 19, 104, DateTimeKind.Local).AddTicks(64), true, null, "Beef" },
                    { new Guid("25903846-3144-450d-9d05-1243c68ff4c7"), new DateTime(2025, 6, 7, 19, 34, 19, 104, DateTimeKind.Local).AddTicks(73), true, null, "Seafood" },
                    { new Guid("461c6f5c-8275-4e15-bad6-5f05ec18118e"), new DateTime(2025, 6, 7, 19, 34, 19, 104, DateTimeKind.Local).AddTicks(75), true, null, "Vegetable" },
                    { new Guid("8568fa3e-2163-4dd2-8c59-08fdd34d59be"), new DateTime(2025, 6, 7, 19, 34, 19, 104, DateTimeKind.Local).AddTicks(61), true, null, "Chicken" },
                    { new Guid("a6c5ec1d-0456-4765-943f-9c3ccaf487ea"), new DateTime(2025, 6, 7, 19, 34, 19, 104, DateTimeKind.Local).AddTicks(66), true, null, "Pork" }
                });

            migrationBuilder.InsertData(
                table: "TypeOfDish",
                columns: new[] { "ID", "CreatedDate", "IsActive", "ModifiedDate", "Name" },
                values: new object[,]
                {
                    { new Guid("0e8b6ef7-2dee-4905-8615-7a21a6408f91"), new DateTime(2025, 6, 7, 19, 34, 19, 103, DateTimeKind.Local).AddTicks(9685), true, null, "Main Dishes" },
                    { new Guid("2e12a104-da8f-44fd-ac99-4d8407743b70"), new DateTime(2025, 6, 7, 19, 34, 19, 103, DateTimeKind.Local).AddTicks(9687), true, null, "Vegetarian Main Dishes" },
                    { new Guid("7e5a6b23-17aa-4234-a4d2-6ea2231e8de9"), new DateTime(2025, 6, 7, 19, 34, 19, 103, DateTimeKind.Local).AddTicks(9693), true, null, "Healthy Main Dishes" },
                    { new Guid("8e21bb78-8aac-47c5-8fa5-4d326d1b57cd"), new DateTime(2025, 6, 7, 19, 34, 19, 103, DateTimeKind.Local).AddTicks(9662), true, null, "Quick and Easy Dinners for One" },
                    { new Guid("ebf09c3e-adf2-4f4b-9730-e28dac9f4f11"), new DateTime(2025, 6, 7, 19, 34, 19, 103, DateTimeKind.Local).AddTicks(9665), true, null, "Cooking for Two" },
                    { new Guid("ed096fb1-c087-4399-bf3c-cd4a9d8bb65a"), new DateTime(2025, 6, 7, 19, 34, 19, 103, DateTimeKind.Local).AddTicks(9690), true, null, "Side Dishes" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("04e1d6cf-5c29-452d-803f-d62cb7ba5570"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("0c3df224-5b6b-4e9b-85fd-a2a8c3d258bb"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("25903846-3144-450d-9d05-1243c68ff4c7"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("461c6f5c-8275-4e15-bad6-5f05ec18118e"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("8568fa3e-2163-4dd2-8c59-08fdd34d59be"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("a6c5ec1d-0456-4765-943f-9c3ccaf487ea"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("0e8b6ef7-2dee-4905-8615-7a21a6408f91"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("2e12a104-da8f-44fd-ac99-4d8407743b70"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("7e5a6b23-17aa-4234-a4d2-6ea2231e8de9"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("8e21bb78-8aac-47c5-8fa5-4d326d1b57cd"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("ebf09c3e-adf2-4f4b-9730-e28dac9f4f11"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("ed096fb1-c087-4399-bf3c-cd4a9d8bb65a"));

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Vouchers");

            migrationBuilder.DropColumn(
                name: "MaxDiscountAmount",
                table: "Vouchers");

            migrationBuilder.DropColumn(
                name: "UsagePerUser",
                table: "Vouchers");

            migrationBuilder.DropColumn(
                name: "RejectNote",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RejectNote",
                table: "StoreDetails");

            migrationBuilder.DropColumn(
                name: "RejectNote",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "status",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "view",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "RejectNote",
                table: "Complaints");

            migrationBuilder.DropColumn(
                name: "RejectNote",
                table: "BalanceChanges");

            migrationBuilder.AddColumn<string>(
                name: "Scope",
                table: "Vouchers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "IngredientTag",
                columns: new[] { "ID", "CreatedDate", "IsActive", "ModifiedDate", "Name" },
                values: new object[,]
                {
                    { new Guid("02aecfaa-e550-416a-ac68-b79105abb772"), new DateTime(2025, 6, 3, 16, 59, 4, 850, DateTimeKind.Local).AddTicks(3546), true, null, "Pork" },
                    { new Guid("1523546b-43ca-470d-bdf3-064b68cf8327"), new DateTime(2025, 6, 3, 16, 59, 4, 850, DateTimeKind.Local).AddTicks(3537), true, null, "Chicken" },
                    { new Guid("174c4ccf-82ba-43e4-bd4a-7ee12491542e"), new DateTime(2025, 6, 3, 16, 59, 4, 850, DateTimeKind.Local).AddTicks(3549), true, null, "Seafood" },
                    { new Guid("772cfe90-4054-4ee5-b6ad-a97998b07b05"), new DateTime(2025, 6, 3, 16, 59, 4, 850, DateTimeKind.Local).AddTicks(3535), true, null, "Fish" },
                    { new Guid("78a548ae-09ef-43eb-a4ee-5c87bec4dacc"), new DateTime(2025, 6, 3, 16, 59, 4, 850, DateTimeKind.Local).AddTicks(3544), true, null, "Beef" },
                    { new Guid("f3708c7a-9ec0-4440-a576-acf378f8f703"), new DateTime(2025, 6, 3, 16, 59, 4, 850, DateTimeKind.Local).AddTicks(3551), true, null, "Vegetable" }
                });

            migrationBuilder.InsertData(
                table: "TypeOfDish",
                columns: new[] { "ID", "CreatedDate", "IsActive", "ModifiedDate", "Name" },
                values: new object[,]
                {
                    { new Guid("14f70984-edee-464c-a1f5-514d56615422"), new DateTime(2025, 6, 3, 16, 59, 4, 850, DateTimeKind.Local).AddTicks(3254), true, null, "Quick and Easy Dinners for One" },
                    { new Guid("308788ef-86c7-449c-a6f0-e6c4f58df002"), new DateTime(2025, 6, 3, 16, 59, 4, 850, DateTimeKind.Local).AddTicks(3266), true, null, "Healthy Main Dishes" },
                    { new Guid("441ee5cb-1b9e-4308-a3a0-4436f6172705"), new DateTime(2025, 6, 3, 16, 59, 4, 850, DateTimeKind.Local).AddTicks(3257), true, null, "Cooking for Two" },
                    { new Guid("6af478fe-bb61-43de-bc2a-98ad069ed6f0"), new DateTime(2025, 6, 3, 16, 59, 4, 850, DateTimeKind.Local).AddTicks(3262), true, null, "Vegetarian Main Dishes" },
                    { new Guid("81c8b8b8-b1b5-4fdb-afd3-f22a9e97917b"), new DateTime(2025, 6, 3, 16, 59, 4, 850, DateTimeKind.Local).AddTicks(3264), true, null, "Side Dishes" },
                    { new Guid("ea20ddfc-75c1-4d59-8482-987c75a3c415"), new DateTime(2025, 6, 3, 16, 59, 4, 850, DateTimeKind.Local).AddTicks(3259), true, null, "Main Dishes" }
                });
        }
    }
}
