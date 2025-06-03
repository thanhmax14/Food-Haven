using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class migrationsv11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecipeIngredientTags");

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("05c95f77-f2a1-4cfc-9250-1996386aede7"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("1a0c4a7e-372a-48c0-baaf-d2414c03b659"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("228e7e73-6802-4e5e-a188-e2c00e5776d6"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("5ff5a9dd-9664-49b3-b60d-f9fedc9b93a4"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("a73f2b3f-a271-4b45-b3e4-ebfd1264abfc"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("fc0b8060-6945-4ced-92fb-130e446fc24a"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("6e4abc7f-4141-4be2-8da4-ecbcf87425e6"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("78f3cc41-e5d7-4f42-9270-2e91f38c08a7"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("90f12565-b92b-4ab3-80a8-15e08083b1ec"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("9c9515c9-1f6d-470f-b23f-6f78877e4ba4"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("c8522c52-7e89-4de1-8fe5-26ba3f2cb6f9"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("f01f726f-161e-4835-aeef-46c3eefe9335"));

            migrationBuilder.CreateTable(
                name: "RecipeIngredientTag",
                columns: table => new
                {
                    RecipeID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IngredientTagID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeIngredientTag", x => new { x.RecipeID, x.IngredientTagID });
                    table.ForeignKey(
                        name: "FK_RecipeIngredientTag_IngredientTag_IngredientTagID",
                        column: x => x.IngredientTagID,
                        principalTable: "IngredientTag",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecipeIngredientTag_Recipes_RecipeID",
                        column: x => x.RecipeID,
                        principalTable: "Recipes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_RecipeIngredientTag_IngredientTagID",
                table: "RecipeIngredientTag",
                column: "IngredientTagID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecipeIngredientTag");

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
                    { new Guid("05c95f77-f2a1-4cfc-9250-1996386aede7"), new DateTime(2025, 6, 3, 11, 42, 55, 24, DateTimeKind.Local).AddTicks(8063), true, null, "Pork" },
                    { new Guid("1a0c4a7e-372a-48c0-baaf-d2414c03b659"), new DateTime(2025, 6, 3, 11, 42, 55, 24, DateTimeKind.Local).AddTicks(8060), true, null, "Beef" },
                    { new Guid("228e7e73-6802-4e5e-a188-e2c00e5776d6"), new DateTime(2025, 6, 3, 11, 42, 55, 24, DateTimeKind.Local).AddTicks(8054), true, null, "Fish" },
                    { new Guid("5ff5a9dd-9664-49b3-b60d-f9fedc9b93a4"), new DateTime(2025, 6, 3, 11, 42, 55, 24, DateTimeKind.Local).AddTicks(8057), true, null, "Chicken" },
                    { new Guid("a73f2b3f-a271-4b45-b3e4-ebfd1264abfc"), new DateTime(2025, 6, 3, 11, 42, 55, 24, DateTimeKind.Local).AddTicks(8068), true, null, "Vegetable" },
                    { new Guid("fc0b8060-6945-4ced-92fb-130e446fc24a"), new DateTime(2025, 6, 3, 11, 42, 55, 24, DateTimeKind.Local).AddTicks(8065), true, null, "Seafood" }
                });

            migrationBuilder.InsertData(
                table: "TypeOfDish",
                columns: new[] { "ID", "CreatedDate", "IsActive", "ModifiedDate", "Name" },
                values: new object[,]
                {
                    { new Guid("6e4abc7f-4141-4be2-8da4-ecbcf87425e6"), new DateTime(2025, 6, 3, 11, 42, 55, 24, DateTimeKind.Local).AddTicks(7603), true, null, "Main Dishes" },
                    { new Guid("78f3cc41-e5d7-4f42-9270-2e91f38c08a7"), new DateTime(2025, 6, 3, 11, 42, 55, 24, DateTimeKind.Local).AddTicks(7600), true, null, "Cooking for Two" },
                    { new Guid("90f12565-b92b-4ab3-80a8-15e08083b1ec"), new DateTime(2025, 6, 3, 11, 42, 55, 24, DateTimeKind.Local).AddTicks(7648), true, null, "Healthy Main Dishes" },
                    { new Guid("9c9515c9-1f6d-470f-b23f-6f78877e4ba4"), new DateTime(2025, 6, 3, 11, 42, 55, 24, DateTimeKind.Local).AddTicks(7597), true, null, "Quick and Easy Dinners for One" },
                    { new Guid("c8522c52-7e89-4de1-8fe5-26ba3f2cb6f9"), new DateTime(2025, 6, 3, 11, 42, 55, 24, DateTimeKind.Local).AddTicks(7608), true, null, "Side Dishes" },
                    { new Guid("f01f726f-161e-4835-aeef-46c3eefe9335"), new DateTime(2025, 6, 3, 11, 42, 55, 24, DateTimeKind.Local).AddTicks(7605), true, null, "Vegetarian Main Dishes" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecipeIngredientTags_RecipesID",
                table: "RecipeIngredientTags",
                column: "RecipesID");
        }
    }
}
