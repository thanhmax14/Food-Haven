using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class migrationsv15 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StoreFollowers");

            migrationBuilder.DropTable(
                name: "StoreReports");

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("26631dd2-cb66-4467-8825-96e5cee81c4b"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("32797bb0-9971-4035-ab30-0fac3f31907b"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("37bdf07d-6f2c-4cde-b5c8-a55d5c048f49"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("415b1190-57d9-4622-abb9-02e78adb395d"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("97e16152-9605-4503-a11b-bf5fa9839c21"));

            migrationBuilder.DeleteData(
                table: "IngredientTag",
                keyColumn: "ID",
                keyValue: new Guid("cbda0ee9-1bd9-4c30-9426-8d4f1454111b"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("13dedd2c-c681-4a39-88ca-b6c773704eb0"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("2542a75a-0395-4436-b039-b63a26705853"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("258be550-e959-48be-af41-99918cebbfb9"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("53cf5a0d-9bd7-4f2e-b8d6-a946ea4c539e"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("62332def-2bdc-4e1b-804e-0fa45be6b750"));

            migrationBuilder.DeleteData(
                table: "TypeOfDish",
                keyColumn: "ID",
                keyValue: new Guid("e5c249f7-dc50-48ee-98a4-2466c3f345b8"));

            migrationBuilder.AlterColumn<string>(
                name: "DiscountType",
                table: "Vouchers",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "RequestSeller",
                table: "Users",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "Users",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Users",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Users",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Orders",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "PaymentStatus",
                table: "Orders",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "PaymentMethod",
                table: "Orders",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Orders",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "DeliveryAddress",
                table: "Orders",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "OrderDetails",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Reply",
                table: "Complaints",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RejectNote",
                table: "Complaints",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Complaints",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "AdminReportStatus",
                table: "Complaints",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AdminReply",
                table: "Complaints",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RejectNote",
                table: "BalanceChanges",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "BalanceChanges",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FromUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ToUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MessageText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRepliedToMessageID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    HasDropDown = table.Column<bool>(type: "bit", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    ReadAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Messages_Messages_IsRepliedToMessageID",
                        column: x => x.IsRepliedToMessageID,
                        principalTable: "Messages",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Messages_Users_FromUserId",
                        column: x => x.FromUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Messages_Users_ToUserId",
                        column: x => x.ToUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MessageImages",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MessageID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageImages", x => x.ID);
                    table.ForeignKey(
                        name: "FK_MessageImages_Messages_MessageID",
                        column: x => x.MessageID,
                        principalTable: "Messages",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_MessageImages_MessageID",
                table: "MessageImages",
                column: "MessageID");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_FromUserId",
                table: "Messages",
                column: "FromUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_IsRepliedToMessageID",
                table: "Messages",
                column: "IsRepliedToMessageID");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ToUserId",
                table: "Messages",
                column: "ToUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MessageImages");

            migrationBuilder.DropTable(
                name: "Messages");

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

            migrationBuilder.AlterColumn<string>(
                name: "DiscountType",
                table: "Vouchers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "RequestSeller",
                table: "Users",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Orders",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<string>(
                name: "PaymentStatus",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "PaymentMethod",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "DeliveryAddress",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "OrderDetails",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Reply",
                table: "Complaints",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RejectNote",
                table: "Complaints",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Complaints",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "AdminReportStatus",
                table: "Complaints",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AdminReply",
                table: "Complaints",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RejectNote",
                table: "BalanceChanges",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "BalanceChanges",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

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
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
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
                    { new Guid("26631dd2-cb66-4467-8825-96e5cee81c4b"), new DateTime(2025, 6, 15, 15, 16, 17, 273, DateTimeKind.Local).AddTicks(9926), true, null, "Chicken" },
                    { new Guid("32797bb0-9971-4035-ab30-0fac3f31907b"), new DateTime(2025, 6, 15, 15, 16, 17, 273, DateTimeKind.Local).AddTicks(9922), true, null, "Fish" },
                    { new Guid("37bdf07d-6f2c-4cde-b5c8-a55d5c048f49"), new DateTime(2025, 6, 15, 15, 16, 17, 273, DateTimeKind.Local).AddTicks(9933), true, null, "Pork" },
                    { new Guid("415b1190-57d9-4622-abb9-02e78adb395d"), new DateTime(2025, 6, 15, 15, 16, 17, 273, DateTimeKind.Local).AddTicks(9937), true, null, "Seafood" },
                    { new Guid("97e16152-9605-4503-a11b-bf5fa9839c21"), new DateTime(2025, 6, 15, 15, 16, 17, 273, DateTimeKind.Local).AddTicks(9940), true, null, "Vegetable" },
                    { new Guid("cbda0ee9-1bd9-4c30-9426-8d4f1454111b"), new DateTime(2025, 6, 15, 15, 16, 17, 273, DateTimeKind.Local).AddTicks(9929), true, null, "Beef" }
                });

            migrationBuilder.InsertData(
                table: "TypeOfDish",
                columns: new[] { "ID", "CreatedDate", "IsActive", "ModifiedDate", "Name" },
                values: new object[,]
                {
                    { new Guid("13dedd2c-c681-4a39-88ca-b6c773704eb0"), new DateTime(2025, 6, 15, 15, 16, 17, 273, DateTimeKind.Local).AddTicks(9449), true, null, "Main Dishes" },
                    { new Guid("2542a75a-0395-4436-b039-b63a26705853"), new DateTime(2025, 6, 15, 15, 16, 17, 273, DateTimeKind.Local).AddTicks(9452), true, null, "Vegetarian Main Dishes" },
                    { new Guid("258be550-e959-48be-af41-99918cebbfb9"), new DateTime(2025, 6, 15, 15, 16, 17, 273, DateTimeKind.Local).AddTicks(9456), true, null, "Side Dishes" },
                    { new Guid("53cf5a0d-9bd7-4f2e-b8d6-a946ea4c539e"), new DateTime(2025, 6, 15, 15, 16, 17, 273, DateTimeKind.Local).AddTicks(9475), true, null, "Healthy Main Dishes" },
                    { new Guid("62332def-2bdc-4e1b-804e-0fa45be6b750"), new DateTime(2025, 6, 15, 15, 16, 17, 273, DateTimeKind.Local).AddTicks(9440), true, null, "Quick and Easy Dinners for One" },
                    { new Guid("e5c249f7-dc50-48ee-98a4-2466c3f345b8"), new DateTime(2025, 6, 15, 15, 16, 17, 273, DateTimeKind.Local).AddTicks(9445), true, null, "Cooking for Two" }
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
    }
}
