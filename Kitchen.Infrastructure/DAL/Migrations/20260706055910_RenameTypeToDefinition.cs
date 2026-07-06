using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kitchen.Infrastructure.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RenameTypeToDefinition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockItems_ProductDefinitions_TypeName",
                table: "StockItems");

            migrationBuilder.RenameColumn(
                name: "TypeName",
                table: "StockItems",
                newName: "DefinitionName");

            migrationBuilder.RenameIndex(
                name: "IX_StockItems_TypeName",
                table: "StockItems",
                newName: "IX_StockItems_DefinitionName");

            migrationBuilder.AddForeignKey(
                name: "FK_StockItems_ProductDefinitions_DefinitionName",
                table: "StockItems",
                column: "DefinitionName",
                principalTable: "ProductDefinitions",
                principalColumn: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockItems_ProductDefinitions_DefinitionName",
                table: "StockItems");

            migrationBuilder.RenameColumn(
                name: "DefinitionName",
                table: "StockItems",
                newName: "TypeName");

            migrationBuilder.RenameIndex(
                name: "IX_StockItems_DefinitionName",
                table: "StockItems",
                newName: "IX_StockItems_TypeName");

            migrationBuilder.AddForeignKey(
                name: "FK_StockItems_ProductDefinitions_TypeName",
                table: "StockItems",
                column: "TypeName",
                principalTable: "ProductDefinitions",
                principalColumn: "Name");
        }
    }
}
