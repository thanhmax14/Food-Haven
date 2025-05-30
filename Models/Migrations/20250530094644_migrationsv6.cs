using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class migrationsv6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CookTime",
                table: "Recipes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "IngredientTagID",
                table: "Recipes",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "IngredientTag",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IngredientTag", x => x.ID);
                });

            migrationBuilder.InsertData(
                table: "IngredientTag",
                columns: new[] { "ID", "CreatedDate", "IsActive", "ModifiedDate", "Name" },
                values: new object[,]
                {
                    { new Guid("36f5572b-eae1-4286-8a00-b20967b35201"), new DateTime(2025, 5, 30, 16, 46, 42, 945, DateTimeKind.Local).AddTicks(5743), true, null, "Vegetable" },
                    { new Guid("728ab7f7-24ae-4150-b742-26419c80f4e0"), new DateTime(2025, 5, 30, 16, 46, 42, 945, DateTimeKind.Local).AddTicks(5717), true, null, "Chicken" },
                    { new Guid("812f784a-46e0-4d0c-9fed-d5c77bd8c21d"), new DateTime(2025, 5, 30, 16, 46, 42, 945, DateTimeKind.Local).AddTicks(5740), true, null, "Seafood" },
                    { new Guid("ab63850a-86b3-45e6-a92b-100cb069d413"), new DateTime(2025, 5, 30, 16, 46, 42, 945, DateTimeKind.Local).AddTicks(5732), true, null, "Beef" },
                    { new Guid("eeea2886-635e-4e79-b341-303f69b18a53"), new DateTime(2025, 5, 30, 16, 46, 42, 945, DateTimeKind.Local).AddTicks(5736), true, null, "Pork" },
                    { new Guid("ff0a52d3-59ec-404d-b15c-d39b28ab25d1"), new DateTime(2025, 5, 30, 16, 46, 42, 945, DateTimeKind.Local).AddTicks(5713), true, null, "Fish" }
                });

            migrationBuilder.InsertData(
                table: "TypeOfDish",
                columns: new[] { "ID", "CreatedDate", "IsActive", "ModifiedDate", "Name" },
                values: new object[,]
                {
                    { new Guid("3c5aae45-5e1f-42d5-aa1e-ffb9f00b7537"), new DateTime(2025, 5, 30, 16, 46, 42, 945, DateTimeKind.Local).AddTicks(5082), true, null, "Quick and Easy Dinners for One" },
                    { new Guid("3f596692-513b-4daf-bd71-fdf330f49888"), new DateTime(2025, 5, 30, 16, 46, 42, 945, DateTimeKind.Local).AddTicks(5100), true, null, "Side Dishes" },
                    { new Guid("55638ab6-2ce3-449c-8469-014f2be32581"), new DateTime(2025, 5, 30, 16, 46, 42, 945, DateTimeKind.Local).AddTicks(5104), true, null, "Healthy Main Dishes" },
                    { new Guid("5749644a-4f1e-459c-9c44-0e18181823e4"), new DateTime(2025, 5, 30, 16, 46, 42, 945, DateTimeKind.Local).AddTicks(5088), true, null, "Cooking for Two" },
                    { new Guid("8f0e1a8c-70cf-43ec-b8e7-7543bbaeb1f1"), new DateTime(2025, 5, 30, 16, 46, 42, 945, DateTimeKind.Local).AddTicks(5092), true, null, "Main Dishes" },
                    { new Guid("aed30726-bfec-4f82-8af1-2327c909a21c"), new DateTime(2025, 5, 30, 16, 46, 42, 945, DateTimeKind.Local).AddTicks(5096), true, null, "Vegetarian Main Dishes" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_IngredientTagID",
                table: "Recipes",
                column: "IngredientTagID");

            migrationBuilder.AddForeignKey(
                name: "FK_Recipes_IngredientTag_IngredientTagID",
                table: "Recipes",
                column: "IngredientTagID",
                principalTable: "IngredientTag",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recipes_IngredientTag_IngredientTagID",
                table: "Recipes");

            migrationBuilder.DropTable(
                name: "IngredientTag");

            migrationBuilder.DropIndex(
                name: "IX_Recipes_IngredientTagID",
                table: "Recipes");

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("3c5aae45-5e1f-42d5-aa1e-ffb9f00b7537"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("3f596692-513b-4daf-bd71-fdf330f49888"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("55638ab6-2ce3-449c-8469-014f2be32581"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("5749644a-4f1e-459c-9c44-0e18181823e4"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("8f0e1a8c-70cf-43ec-b8e7-7543bbaeb1f1"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("aed30726-bfec-4f82-8af1-2327c909a21c"));

            migrationBuilder.DropColumn(
                name: "CookTime",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "IngredientTagID",
                table: "Recipes");
        }
    }
}
