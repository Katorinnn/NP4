using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LPG_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class RemovedQuantityinInventory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "tbl_inventory");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "tbl_inventory",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
