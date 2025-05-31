using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class migrationsv7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recipes_IngredientTag_IngredientTagID",
                table: "Recipes");

            migrationBuilder.DropIndex(
                name: "IX_Recipes_IngredientTagID",
                table: "Recipes");

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("36f5572b-eae1-4286-8a00-b20967b35201"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("728ab7f7-24ae-4150-b742-26419c80f4e0"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("812f784a-46e0-4d0c-9fed-d5c77bd8c21d"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("ab63850a-86b3-45e6-a92b-100cb069d413"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("eeea2886-635e-4e79-b341-303f69b18a53"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("ff0a52d3-59ec-404d-b15c-d39b28ab25d1"));

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
                name: "IngredientTagID",
                table: "Recipes");

            migrationBuilder.CreateTable(
                name: "RecipeIngredientTags",
                columns: table => new
                {
                    IngredientTagsID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RecipesID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeIngredientTags", x => new { x.IngredientTagsID, x.RecipesID });
                    table.ForeignKey(
                        name: "FK_RecipeIngredientTags_IngredientTag_IngredientTagsID",
                        column: x => x.IngredientTagsID,
                        principalTable: "IngredientTag",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecipeIngredientTags_Recipes_RecipesID",
                        column: x => x.RecipesID,
                        principalTable: "Recipes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "IngredientTag",
                columns: new[] { "ID", "CreatedDate", "IsActive", "ModifiedDate", "Name" },
                values: new object[,]
                {
                    { new Guid("03adc024-ac3b-4bcc-823a-8571f4bccc80"), new DateTime(2025, 5, 31, 22, 26, 42, 136, DateTimeKind.Local).AddTicks(3490), true, null, "Fish" },
                    { new Guid("1d514df3-2e41-46c8-83c1-4b9b296c3441"), new DateTime(2025, 5, 31, 22, 26, 42, 136, DateTimeKind.Local).AddTicks(3494), true, null, "Chicken" },
                    { new Guid("64f424bc-41b0-4454-aa4d-408b8b429fb1"), new DateTime(2025, 5, 31, 22, 26, 42, 136, DateTimeKind.Local).AddTicks(3506), true, null, "Vegetable" },
                    { new Guid("7fc99b4c-cd26-42d5-8f12-3c49bc2bba39"), new DateTime(2025, 5, 31, 22, 26, 42, 136, DateTimeKind.Local).AddTicks(3503), true, null, "Seafood" },
                    { new Guid("dbe1a64d-511b-4ffc-a82b-0eb40abc989b"), new DateTime(2025, 5, 31, 22, 26, 42, 136, DateTimeKind.Local).AddTicks(3497), true, null, "Beef" },
                    { new Guid("f0382441-edaa-48b5-8038-e0e9b7e195d1"), new DateTime(2025, 5, 31, 22, 26, 42, 136, DateTimeKind.Local).AddTicks(3500), true, null, "Pork" }
                });

            migrationBuilder.InsertData(
                table: "TypeOfDish",
                columns: new[] { "ID", "CreatedDate", "IsActive", "ModifiedDate", "Name" },
                values: new object[,]
                {
                    { new Guid("1f4de843-cbc1-46b0-8435-9f30e3f7498e"), new DateTime(2025, 5, 31, 22, 26, 42, 136, DateTimeKind.Local).AddTicks(3153), true, null, "Cooking for Two" },
                    { new Guid("245459d2-2e6d-4098-a2e7-85fae5c3cc96"), new DateTime(2025, 5, 31, 22, 26, 42, 136, DateTimeKind.Local).AddTicks(3162), true, null, "Side Dishes" },
                    { new Guid("456442ed-c9e8-45c5-adc3-72144455df31"), new DateTime(2025, 5, 31, 22, 26, 42, 136, DateTimeKind.Local).AddTicks(3165), true, null, "Healthy Main Dishes" },
                    { new Guid("6afc95e4-1efb-49f2-b9c1-a32c345c2a64"), new DateTime(2025, 5, 31, 22, 26, 42, 136, DateTimeKind.Local).AddTicks(3150), true, null, "Quick and Easy Dinners for One" },
                    { new Guid("84a1f740-4ad2-4b39-9e23-e1c751e70c3a"), new DateTime(2025, 5, 31, 22, 26, 42, 136, DateTimeKind.Local).AddTicks(3156), true, null, "Main Dishes" },
                    { new Guid("e2871ae1-fe0b-4192-b939-a17770df6d82"), new DateTime(2025, 5, 31, 22, 26, 42, 136, DateTimeKind.Local).AddTicks(3160), true, null, "Vegetarian Main Dishes" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecipeIngredientTags_RecipesID",
                table: "RecipeIngredientTags",
                column: "RecipesID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecipeIngredientTags");

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("03adc024-ac3b-4bcc-823a-8571f4bccc80"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("1d514df3-2e41-46c8-83c1-4b9b296c3441"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("64f424bc-41b0-4454-aa4d-408b8b429fb1"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("7fc99b4c-cd26-42d5-8f12-3c49bc2bba39"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("dbe1a64d-511b-4ffc-a82b-0eb40abc989b"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("f0382441-edaa-48b5-8038-e0e9b7e195d1"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("1f4de843-cbc1-46b0-8435-9f30e3f7498e"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("245459d2-2e6d-4098-a2e7-85fae5c3cc96"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("456442ed-c9e8-45c5-adc3-72144455df31"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("6afc95e4-1efb-49f2-b9c1-a32c345c2a64"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("84a1f740-4ad2-4b39-9e23-e1c751e70c3a"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("e2871ae1-fe0b-4192-b939-a17770df6d82"));

            migrationBuilder.AddColumn<Guid>(
                name: "IngredientTagID",
                table: "Recipes",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

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
    }
}
