using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LPG_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class KeyBackinTransactin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_tbl_reports",
                table: "tbl_reports");

            migrationBuilder.DropColumn(
                name: "Transaction",
                table: "tbl_reports");

            migrationBuilder.AlterColumn<int>(
                name: "TransactionID",
                table: "tbl_reports",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_tbl_reports",
                table: "tbl_reports",
                column: "TransactionID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_tbl_reports",
                table: "tbl_reports");

            migrationBuilder.AlterColumn<int>(
                name: "TransactionID",
                table: "tbl_reports",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<int>(
                name: "Transaction",
                table: "tbl_reports",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0)
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_tbl_reports",
                table: "tbl_reports",
                column: "Transaction");
        }
    }
}
