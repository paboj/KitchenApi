using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kitchen.Infrastructure.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddIngredientTypeLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TypeName",
                table: "Ingredients",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ingredients_TypeName",
                table: "Ingredients",
                column: "TypeName");

            migrationBuilder.AddForeignKey(
                name: "FK_Ingredients_IngredientTypes_TypeName",
                table: "Ingredients",
                column: "TypeName",
                principalTable: "IngredientTypes",
                principalColumn: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ingredients_IngredientTypes_TypeName",
                table: "Ingredients");

            migrationBuilder.DropIndex(
                name: "IX_Ingredients_TypeName",
                table: "Ingredients");

            migrationBuilder.DropColumn(
                name: "TypeName",
                table: "Ingredients");
        }
    }
}
