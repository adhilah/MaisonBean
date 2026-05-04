using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MaisonBean.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomizationToCart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Strength",
                table: "CartItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Sweetness",
                table: "CartItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Temp",
                table: "CartItems",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Strength",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "Sweetness",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "Temp",
                table: "CartItems");
        }
    }
}
