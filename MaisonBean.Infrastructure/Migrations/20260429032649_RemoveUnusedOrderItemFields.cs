using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MaisonBean.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUnusedOrderItemFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BasePrice",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "BeanName",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "MilkName",
                table: "OrderItems");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "BasePrice",
                table: "OrderItems",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "BeanName",
                table: "OrderItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MilkName",
                table: "OrderItems",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
