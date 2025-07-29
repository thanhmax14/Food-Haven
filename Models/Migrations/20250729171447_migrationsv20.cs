using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class migrationsv20 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<bool>(
                name: "IsProductTypeBanned",
                table: "ProductTypes",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsProductBanned",
                table: "Products",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPaid",
                table: "Orders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "ExpertRecipe",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Ingredients = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Directions = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Link = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Source = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NER = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpertRecipe", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "RecipeViewHistory",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserID = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    ExpertRecipeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ViewedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MatchedIngredients = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeViewHistory", x => x.ID);
                    table.ForeignKey(
                        name: "FK_RecipeViewHistory_ExpertRecipe_ExpertRecipeId",
                        column: x => x.ExpertRecipeId,
                        principalTable: "ExpertRecipe",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecipeViewHistory_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_RecipeViewHistory_ExpertRecipeId",
                table: "RecipeViewHistory",
                column: "ExpertRecipeId");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeViewHistory_UserID",
                table: "RecipeViewHistory",
                column: "UserID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecipeViewHistory");

            migrationBuilder.DropTable(
                name: "ExpertRecipe");

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

            migrationBuilder.DropColumn(
                name: "IsProductTypeBanned",
                table: "ProductTypes");

            migrationBuilder.DropColumn(
                name: "IsProductBanned",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "IsPaid",
                table: "Orders");

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
    }
}
