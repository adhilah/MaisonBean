using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MaisonBean.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsBlockedToBeanAndMilk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsBlocked",
                table: "MilkOptions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsBlocked",
                table: "BeanTypes",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBlocked",
                table: "MilkOptions");

            migrationBuilder.DropColumn(
                name: "IsBlocked",
                table: "BeanTypes");
        }
    }
}
