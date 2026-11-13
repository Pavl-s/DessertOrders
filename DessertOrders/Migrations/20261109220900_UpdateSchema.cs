using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DessertOrders.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Desserts",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Ingredients",
                table: "Desserts",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "Desserts",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Stock",
                table: "Desserts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Weight",
                table: "Desserts",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Desserts");

            migrationBuilder.DropColumn(
                name: "Ingredients",
                table: "Desserts");

            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "Desserts");

            migrationBuilder.DropColumn(
                name: "Stock",
                table: "Desserts");

            migrationBuilder.DropColumn(
                name: "Weight",
                table: "Desserts");
        }
    }
}
