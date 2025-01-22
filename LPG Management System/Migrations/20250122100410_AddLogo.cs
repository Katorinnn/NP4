using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LPG_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class AddLogo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "Logo",
                table: "tbl_admin",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Logo",
                table: "tbl_admin");
        }
    }
}
