using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LPG_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class LogomovedtoCompanyTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Logo",
                table: "tbl_admin");

            migrationBuilder.AddColumn<byte[]>(
                name: "Logo",
                table: "tbl_company",
                type: "BLOB",
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Logo",
                table: "tbl_company");

            migrationBuilder.AddColumn<byte[]>(
                name: "Logo",
                table: "tbl_admin",
                type: "BLOB",
                nullable: false,
                defaultValue: new byte[0]);
        }
    }
}
