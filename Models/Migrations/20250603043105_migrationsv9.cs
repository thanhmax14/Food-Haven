using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class migrationsv9 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("236c5e2a-9adc-4cd8-8363-a25d32c8a8aa"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("55d468f8-cb33-44c9-bc88-1c34df3dc7f0"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("57f2a1a9-b505-404d-9e28-f0010786aabc"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("6fe964a2-e704-4e92-8f48-6d8c23e66cc2"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("d98b018e-39d9-4c25-9aae-24442e689643"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("fb8a8625-2b85-4328-8662-53880f1c7060"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("0ffed563-46ee-4f1b-98c6-b77a3132f88b"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("944a3eaf-7c44-41f1-9f13-3ae79bd3ace2"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("96352474-dcc3-44b0-8e5b-691bf6afdf36"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("b2088fef-c026-4c7a-a75e-36bb220e84e3"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("db2971bb-569c-4585-a744-54a59d04a856"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("fd5f4aa2-f838-4e25-9af7-a28ae21f653f"));

            migrationBuilder.AddColumn<string>(
                name: "AdminReply",
                table: "Complaints",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdminReportStatus",
                table: "Complaints",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateAdminReply",
                table: "Complaints",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsReportAdmin",
                table: "Complaints",
                type: "bit",
                nullable: false,
                defaultValue: false);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "AdminReply",
                table: "Complaints");

            migrationBuilder.DropColumn(
                name: "AdminReportStatus",
                table: "Complaints");

            migrationBuilder.DropColumn(
                name: "DateAdminReply",
                table: "Complaints");

            migrationBuilder.DropColumn(
                name: "IsReportAdmin",
                table: "Complaints");

            migrationBuilder.InsertData(
                table: "IngredientTag",
                columns: new[] { "ID", "CreatedDate", "IsActive", "ModifiedDate", "Name" },
                values: new object[,]
                {
                    { new Guid("236c5e2a-9adc-4cd8-8363-a25d32c8a8aa"), new DateTime(2025, 6, 2, 12, 8, 34, 497, DateTimeKind.Local).AddTicks(7872), true, null, "Seafood" },
                    { new Guid("55d468f8-cb33-44c9-bc88-1c34df3dc7f0"), new DateTime(2025, 6, 2, 12, 8, 34, 497, DateTimeKind.Local).AddTicks(7875), true, null, "Vegetable" },
                    { new Guid("57f2a1a9-b505-404d-9e28-f0010786aabc"), new DateTime(2025, 6, 2, 12, 8, 34, 497, DateTimeKind.Local).AddTicks(7869), true, null, "Pork" },
                    { new Guid("6fe964a2-e704-4e92-8f48-6d8c23e66cc2"), new DateTime(2025, 6, 2, 12, 8, 34, 497, DateTimeKind.Local).AddTicks(7865), true, null, "Beef" },
                    { new Guid("d98b018e-39d9-4c25-9aae-24442e689643"), new DateTime(2025, 6, 2, 12, 8, 34, 497, DateTimeKind.Local).AddTicks(7848), true, null, "Fish" },
                    { new Guid("fb8a8625-2b85-4328-8662-53880f1c7060"), new DateTime(2025, 6, 2, 12, 8, 34, 497, DateTimeKind.Local).AddTicks(7851), true, null, "Chicken" }
                });

            migrationBuilder.InsertData(
                table: "TypeOfDish",
                columns: new[] { "ID", "CreatedDate", "IsActive", "ModifiedDate", "Name" },
                values: new object[,]
                {
                    { new Guid("0ffed563-46ee-4f1b-98c6-b77a3132f88b"), new DateTime(2025, 6, 2, 12, 8, 34, 497, DateTimeKind.Local).AddTicks(7462), true, null, "Healthy Main Dishes" },
                    { new Guid("944a3eaf-7c44-41f1-9f13-3ae79bd3ace2"), new DateTime(2025, 6, 2, 12, 8, 34, 497, DateTimeKind.Local).AddTicks(7440), true, null, "Side Dishes" },
                    { new Guid("96352474-dcc3-44b0-8e5b-691bf6afdf36"), new DateTime(2025, 6, 2, 12, 8, 34, 497, DateTimeKind.Local).AddTicks(7431), true, null, "Cooking for Two" },
                    { new Guid("b2088fef-c026-4c7a-a75e-36bb220e84e3"), new DateTime(2025, 6, 2, 12, 8, 34, 497, DateTimeKind.Local).AddTicks(7434), true, null, "Main Dishes" },
                    { new Guid("db2971bb-569c-4585-a744-54a59d04a856"), new DateTime(2025, 6, 2, 12, 8, 34, 497, DateTimeKind.Local).AddTicks(7428), true, null, "Quick and Easy Dinners for One" },
                    { new Guid("fd5f4aa2-f838-4e25-9af7-a28ae21f653f"), new DateTime(2025, 6, 2, 12, 8, 34, 497, DateTimeKind.Local).AddTicks(7437), true, null, "Vegetarian Main Dishes" }
                });
        }
    }
}
