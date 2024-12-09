using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LPG_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class ReportsTableNewFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReportQuattt",
                table: "tbl_reports",
                newName: "UnitPrice");

            migrationBuilder.RenameColumn(
                name: "ReportDetails",
                table: "tbl_reports",
                newName: "TotalPrice");

            migrationBuilder.RenameColumn(
                name: "ReportId",
                table: "tbl_reports",
                newName: "TransactionID");

            migrationBuilder.AddColumn<decimal>(
                name: "ChangeGiven",
                table: "tbl_reports",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "tbl_reports",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "PaidAmount",
                table: "tbl_reports",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethod",
                table: "tbl_reports",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProductName",
                table: "tbl_reports",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "tbl_reports",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "tbl_reports",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TankID",
                table: "tbl_reports",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChangeGiven",
                table: "tbl_reports");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "tbl_reports");

            migrationBuilder.DropColumn(
                name: "PaidAmount",
                table: "tbl_reports");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "tbl_reports");

            migrationBuilder.DropColumn(
                name: "ProductName",
                table: "tbl_reports");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "tbl_reports");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "tbl_reports");

            migrationBuilder.DropColumn(
                name: "TankID",
                table: "tbl_reports");

            migrationBuilder.RenameColumn(
                name: "UnitPrice",
                table: "tbl_reports",
                newName: "ReportQuattt");

            migrationBuilder.RenameColumn(
                name: "TotalPrice",
                table: "tbl_reports",
                newName: "ReportDetails");

            migrationBuilder.RenameColumn(
                name: "TransactionID",
                table: "tbl_reports",
                newName: "ReportId");
        }
    }
}
