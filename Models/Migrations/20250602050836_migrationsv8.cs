using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class migrationsv8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.AddColumn<string>(
                name: "FinishedDishImage",
                table: "Recipes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ThumbnailImage",
                table: "Recipes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "ComplaintImage",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ComplaintID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComplaintImage", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ComplaintImage_Complaints_ComplaintID",
                        column: x => x.ComplaintID,
                        principalTable: "Complaints",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_ComplaintImage_ComplaintID",
                table: "ComplaintImage",
                column: "ComplaintID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ComplaintImage");

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

            migrationBuilder.DropColumn(
                name: "FinishedDishImage",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "ThumbnailImage",
                table: "Recipes");

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
        }
    }
}
