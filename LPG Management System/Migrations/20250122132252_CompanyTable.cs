using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LPG_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class CompanyTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tbl_company",
                columns: table => new
                {
                    CompanyID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CompanyName = table.Column<string>(type: "TEXT", nullable: false),
                    CompanyAddress = table.Column<string>(type: "TEXT", nullable: false),
                    CompanyContact = table.Column<string>(type: "TEXT", nullable: false),
                    CompanyEmail = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_company", x => x.CompanyID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbl_company");
        }
    }
}
