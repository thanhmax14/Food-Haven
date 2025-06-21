using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class migrationsv18 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateTable(
                name: "StoreFollowers",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StoreID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserID = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreFollowers", x => x.ID);
                    table.ForeignKey(
                        name: "FK_StoreFollowers_StoreDetails_StoreID",
                        column: x => x.StoreID,
                        principalTable: "StoreDetails",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoreFollowers_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoreReports",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StoreID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserID = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreReports", x => x.ID);
                    table.ForeignKey(
                        name: "FK_StoreReports_StoreDetails_StoreID",
                        column: x => x.StoreID,
                        principalTable: "StoreDetails",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoreReports_Users_UserID",
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

            migrationBuilder.CreateIndex(
                name: "IX_StoreFollowers_StoreID",
                table: "StoreFollowers",
                column: "StoreID");

            migrationBuilder.CreateIndex(
                name: "IX_StoreFollowers_UserID",
                table: "StoreFollowers",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_StoreReports_StoreID",
                table: "StoreReports",
                column: "StoreID");

            migrationBuilder.CreateIndex(
                name: "IX_StoreReports_UserID",
                table: "StoreReports",
                column: "UserID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StoreFollowers");

            migrationBuilder.DropTable(
                name: "StoreReports");

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
    }
}
