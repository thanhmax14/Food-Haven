using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class migrationsv5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LongDescriptions",
                table: "Recipes",
                newName: "TotalTime");

            migrationBuilder.RenameColumn(
                name: "Relay",
                table: "RecipeReviews",
                newName: "Reply");

            migrationBuilder.RenameColumn(
                name: "DateRelay",
                table: "RecipeReviews",
                newName: "ReplyDate");

            migrationBuilder.RenameColumn(
                name: "Cmt",
                table: "RecipeReviews",
                newName: "Comment");

            migrationBuilder.AddColumn<string>(
                name: "CookingStep",
                table: "Recipes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DifficultyLevel",
                table: "Recipes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Ingredient",
                table: "Recipes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PreparationTime",
                table: "Recipes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "RecipesMadeItCount",
                table: "Recipes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Servings",
                table: "Recipes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "TypeOfDishID",
                table: "Recipes",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "TypeOfDish",
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
                    table.PrimaryKey("PK_TypeOfDish", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_TypeOfDishID",
                table: "Recipes",
                column: "TypeOfDishID");

            migrationBuilder.AddForeignKey(
                name: "FK_Recipes_TypeOfDish_TypeOfDishID",
                table: "Recipes",
                column: "TypeOfDishID",
                principalTable: "TypeOfDish",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recipes_TypeOfDish_TypeOfDishID",
                table: "Recipes");

            migrationBuilder.DropTable(
                name: "TypeOfDish");

            migrationBuilder.DropIndex(
                name: "IX_Recipes_TypeOfDishID",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "CookingStep",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "DifficultyLevel",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "Ingredient",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "PreparationTime",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "RecipesMadeItCount",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "Servings",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "TypeOfDishID",
                table: "Recipes");

            migrationBuilder.RenameColumn(
                name: "TotalTime",
                table: "Recipes",
                newName: "LongDescriptions");

            migrationBuilder.RenameColumn(
                name: "ReplyDate",
                table: "RecipeReviews",
                newName: "DateRelay");

            migrationBuilder.RenameColumn(
                name: "Reply",
                table: "RecipeReviews",
                newName: "Relay");

            migrationBuilder.RenameColumn(
                name: "Comment",
                table: "RecipeReviews",
                newName: "Cmt");
        }
    }
}
