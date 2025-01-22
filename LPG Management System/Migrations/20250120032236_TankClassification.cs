using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LPG_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class TankClassification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TankClassification",
                table: "tbl_customers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TankClassification",
                table: "tbl_customers");
        }
    }
}
