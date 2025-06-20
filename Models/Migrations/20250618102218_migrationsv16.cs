using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class migrationsv16 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Messages_IsRepliedToMessageID",
                table: "Messages");

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("1324cfe9-4269-4fa9-9b4c-c7081f3ab7cb"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("1ed3b569-3088-4bea-a3e2-58dd0e1f425a"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("2ab1bc75-6605-4259-b67e-67fa6b07a4da"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("385f9fbe-6ed0-4f2d-b7da-9cb517f8adf3"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("61443c1b-40b2-46db-bb8e-c7bcc6fa2155"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("63f07b81-b1a8-4efc-b94f-1e85c946e8ce"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("2ad6b270-5759-4bb5-88ea-3ef6f676b8c3"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("30fb24c1-d0b9-48ab-920a-f51ea89cee33"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("6025eb07-e3e8-455c-acd7-5322198756fb"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("671e1658-658d-4d24-83d0-886a8d9dea7b"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("a8feabff-9810-4498-ab3e-1ac9a9502fc4"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("d800dc7c-39ec-4c78-bbb0-2d6e18da432c"));

            migrationBuilder.RenameColumn(
                name: "IsRepliedToMessageID",
                table: "Messages",
                newName: "RepliedToMessageId");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_IsRepliedToMessageID",
                table: "Messages",
                newName: "IX_Messages_RepliedToMessageId");

            migrationBuilder.AlterColumn<string>(
                name: "MessageText",
                table: "Messages",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "IngredientTag",
                columns: new[] { "ID", "CreatedDate", "IsActive", "ModifiedDate", "Name" },
                values: new object[,]
                {
                    { new Guid("07551154-2960-42b6-84b9-4d2ad4f6e009"), new DateTime(2025, 6, 18, 17, 22, 16, 806, DateTimeKind.Local).AddTicks(2373), true, null, "Beef" },
                    { new Guid("19141ee9-68ac-4ff1-82fa-cc3b47b9d03b"), new DateTime(2025, 6, 18, 17, 22, 16, 806, DateTimeKind.Local).AddTicks(2378), true, null, "Pork" },
                    { new Guid("97b15834-2126-4d3d-b4b3-cb3ea4203b2c"), new DateTime(2025, 6, 18, 17, 22, 16, 806, DateTimeKind.Local).AddTicks(2338), true, null, "Fish" },
                    { new Guid("a35eb1d4-c720-4669-b1b4-9c75828085aa"), new DateTime(2025, 6, 18, 17, 22, 16, 806, DateTimeKind.Local).AddTicks(2383), true, null, "Seafood" },
                    { new Guid("bff00ebe-9453-4936-b334-f4712a03e580"), new DateTime(2025, 6, 18, 17, 22, 16, 806, DateTimeKind.Local).AddTicks(2387), true, null, "Vegetable" },
                    { new Guid("cb1c72d0-dc75-4088-aa80-6096835b9244"), new DateTime(2025, 6, 18, 17, 22, 16, 806, DateTimeKind.Local).AddTicks(2345), true, null, "Chicken" }
                });

            migrationBuilder.InsertData(
                table: "TypeOfDish",
                columns: new[] { "ID", "CreatedDate", "IsActive", "ModifiedDate", "Name" },
                values: new object[,]
                {
                    { new Guid("662db111-66d0-4469-82a4-4591da0d1286"), new DateTime(2025, 6, 18, 17, 22, 16, 806, DateTimeKind.Local).AddTicks(1631), true, null, "Side Dishes" },
                    { new Guid("9a78507d-0699-4993-a586-b5aa98708190"), new DateTime(2025, 6, 18, 17, 22, 16, 806, DateTimeKind.Local).AddTicks(1627), true, null, "Vegetarian Main Dishes" },
                    { new Guid("a4ca95bc-9b32-47f0-9f2c-2666c0f7a575"), new DateTime(2025, 6, 18, 17, 22, 16, 806, DateTimeKind.Local).AddTicks(1614), true, null, "Quick and Easy Dinners for One" },
                    { new Guid("a8c1adf7-213b-4ed0-91a0-45680c8033c2"), new DateTime(2025, 6, 18, 17, 22, 16, 806, DateTimeKind.Local).AddTicks(1635), true, null, "Healthy Main Dishes" },
                    { new Guid("adfbab0d-af4d-4903-91ce-90a5ca01ae7a"), new DateTime(2025, 6, 18, 17, 22, 16, 806, DateTimeKind.Local).AddTicks(1619), true, null, "Cooking for Two" },
                    { new Guid("aeb56d6d-d7fe-495c-be3c-c29531182523"), new DateTime(2025, 6, 18, 17, 22, 16, 806, DateTimeKind.Local).AddTicks(1623), true, null, "Main Dishes" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Messages_RepliedToMessageId",
                table: "Messages",
                column: "RepliedToMessageId",
                principalTable: "Messages",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Messages_RepliedToMessageId",
                table: "Messages");

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("07551154-2960-42b6-84b9-4d2ad4f6e009"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("19141ee9-68ac-4ff1-82fa-cc3b47b9d03b"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("97b15834-2126-4d3d-b4b3-cb3ea4203b2c"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("a35eb1d4-c720-4669-b1b4-9c75828085aa"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("bff00ebe-9453-4936-b334-f4712a03e580"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("cb1c72d0-dc75-4088-aa80-6096835b9244"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("662db111-66d0-4469-82a4-4591da0d1286"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("9a78507d-0699-4993-a586-b5aa98708190"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("a4ca95bc-9b32-47f0-9f2c-2666c0f7a575"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("a8c1adf7-213b-4ed0-91a0-45680c8033c2"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("adfbab0d-af4d-4903-91ce-90a5ca01ae7a"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("aeb56d6d-d7fe-495c-be3c-c29531182523"));

            migrationBuilder.RenameColumn(
                name: "RepliedToMessageId",
                table: "Messages",
                newName: "IsRepliedToMessageID");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_RepliedToMessageId",
                table: "Messages",
                newName: "IX_Messages_IsRepliedToMessageID");

            migrationBuilder.AlterColumn<string>(
                name: "MessageText",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "IngredientTag",
                columns: new[] { "ID", "CreatedDate", "IsActive", "ModifiedDate", "Name" },
                values: new object[,]
                {
                    { new Guid("1324cfe9-4269-4fa9-9b4c-c7081f3ab7cb"), new DateTime(2025, 6, 18, 17, 12, 12, 877, DateTimeKind.Local).AddTicks(8640), true, null, "Fish" },
                    { new Guid("1ed3b569-3088-4bea-a3e2-58dd0e1f425a"), new DateTime(2025, 6, 18, 17, 12, 12, 877, DateTimeKind.Local).AddTicks(8646), true, null, "Beef" },
                    { new Guid("2ab1bc75-6605-4259-b67e-67fa6b07a4da"), new DateTime(2025, 6, 18, 17, 12, 12, 877, DateTimeKind.Local).AddTicks(8654), true, null, "Vegetable" },
                    { new Guid("385f9fbe-6ed0-4f2d-b7da-9cb517f8adf3"), new DateTime(2025, 6, 18, 17, 12, 12, 877, DateTimeKind.Local).AddTicks(8651), true, null, "Seafood" },
                    { new Guid("61443c1b-40b2-46db-bb8e-c7bcc6fa2155"), new DateTime(2025, 6, 18, 17, 12, 12, 877, DateTimeKind.Local).AddTicks(8643), true, null, "Chicken" },
                    { new Guid("63f07b81-b1a8-4efc-b94f-1e85c946e8ce"), new DateTime(2025, 6, 18, 17, 12, 12, 877, DateTimeKind.Local).AddTicks(8649), true, null, "Pork" }
                });

            migrationBuilder.InsertData(
                table: "TypeOfDish",
                columns: new[] { "ID", "CreatedDate", "IsActive", "ModifiedDate", "Name" },
                values: new object[,]
                {
                    { new Guid("2ad6b270-5759-4bb5-88ea-3ef6f676b8c3"), new DateTime(2025, 6, 18, 17, 12, 12, 877, DateTimeKind.Local).AddTicks(8183), true, null, "Quick and Easy Dinners for One" },
                    { new Guid("30fb24c1-d0b9-48ab-920a-f51ea89cee33"), new DateTime(2025, 6, 18, 17, 12, 12, 877, DateTimeKind.Local).AddTicks(8189), true, null, "Main Dishes" },
                    { new Guid("6025eb07-e3e8-455c-acd7-5322198756fb"), new DateTime(2025, 6, 18, 17, 12, 12, 877, DateTimeKind.Local).AddTicks(8194), true, null, "Side Dishes" },
                    { new Guid("671e1658-658d-4d24-83d0-886a8d9dea7b"), new DateTime(2025, 6, 18, 17, 12, 12, 877, DateTimeKind.Local).AddTicks(8187), true, null, "Cooking for Two" },
                    { new Guid("a8feabff-9810-4498-ab3e-1ac9a9502fc4"), new DateTime(2025, 6, 18, 17, 12, 12, 877, DateTimeKind.Local).AddTicks(8220), true, null, "Healthy Main Dishes" },
                    { new Guid("d800dc7c-39ec-4c78-bbb0-2d6e18da432c"), new DateTime(2025, 6, 18, 17, 12, 12, 877, DateTimeKind.Local).AddTicks(8192), true, null, "Vegetarian Main Dishes" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Messages_IsRepliedToMessageID",
                table: "Messages",
                column: "IsRepliedToMessageID",
                principalTable: "Messages",
                principalColumn: "ID");
        }
    }
}
