using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BOs.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Report_VideoHistory_VideoHistoryID",
                table: "Report");

            migrationBuilder.DropForeignKey(
                name: "FK_VideoLike_Account_AccountID1",
                table: "VideoLike");

            migrationBuilder.DropIndex(
                name: "IX_VideoLike_AccountID1",
                table: "VideoLike");

            migrationBuilder.DropColumn(
                name: "AccountID1",
                table: "VideoLike");

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "VideoView",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "VideoLike",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "VideoHistory",
                type: "datetime",
                nullable: false,
                defaultValueSql: "GETDATE()");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "VideoHistory",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "VideoHistory",
                type: "bit",
                maxLength: 50,
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "StreamAt",
                table: "VideoHistory",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "VideoHistory",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "URL",
                table: "VideoHistory",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "VideoHistory",
                type: "datetime",
                nullable: false,
                defaultValueSql: "GETDATE()");

            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "Share",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "Share",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "SchoolChannel",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "SchoolChannel",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "SchoolChannel",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "SchoolChannel",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "SchoolChannel",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "SchoolChannel",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "SchoolChannel",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");

            migrationBuilder.AddColumn<string>(
                name: "Website",
                table: "SchoolChannel",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndTime",
                table: "Schedule",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "StartTime",
                table: "Schedule",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Schedule",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "Active");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Report",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");

            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "Report",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Program",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");

            migrationBuilder.AddColumn<string>(
                name: "Link",
                table: "Program",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProgramName",
                table: "Program",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Program",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Program",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Program",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");

            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "Payment",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentDate",
                table: "Payment",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethod",
                table: "Payment",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Payment",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Package",
                type: "datetime",
                nullable: false,
                defaultValueSql: "GETDATE()");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Package",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Duration",
                table: "Package",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Package",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Package",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Package",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "True");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Package",
                type: "datetime",
                nullable: false,
                defaultValueSql: "GETDATE()");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "OrderDetail",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "OrderDetail",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Order",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Order",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "TotalPrice",
                table: "Order",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Order",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");

            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "News",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_Report_VideoHistory_VideoHistoryID",
                table: "Report",
                column: "VideoHistoryID",
                principalTable: "VideoHistory",
                principalColumn: "VideoHistoryID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Report_VideoHistory_VideoHistoryID",
                table: "Report");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "VideoView");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "VideoLike");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "VideoHistory");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "VideoHistory");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "VideoHistory");

            migrationBuilder.DropColumn(
                name: "StreamAt",
                table: "VideoHistory");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "VideoHistory");

            migrationBuilder.DropColumn(
                name: "URL",
                table: "VideoHistory");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "VideoHistory");

            migrationBuilder.DropColumn(
                name: "Content",
                table: "Share");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Share");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "SchoolChannel");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "SchoolChannel");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "SchoolChannel");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "SchoolChannel");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "SchoolChannel");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "SchoolChannel");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "SchoolChannel");

            migrationBuilder.DropColumn(
                name: "Website",
                table: "SchoolChannel");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "Schedule");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "Schedule");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Schedule");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Report");

            migrationBuilder.DropColumn(
                name: "Reason",
                table: "Report");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Program");

            migrationBuilder.DropColumn(
                name: "Link",
                table: "Program");

            migrationBuilder.DropColumn(
                name: "ProgramName",
                table: "Program");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Program");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Program");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Program");

            migrationBuilder.DropColumn(
                name: "Amount",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "PaymentDate",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Package");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Package");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "Package");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Package");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Package");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Package");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Package");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "OrderDetail");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "OrderDetail");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "TotalPrice",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "News");

            migrationBuilder.AddColumn<int>(
                name: "AccountID1",
                table: "VideoLike",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_VideoLike_AccountID1",
                table: "VideoLike",
                column: "AccountID1");

            migrationBuilder.AddForeignKey(
                name: "FK_Report_VideoHistory_VideoHistoryID",
                table: "Report",
                column: "VideoHistoryID",
                principalTable: "VideoHistory",
                principalColumn: "VideoHistoryID");

            migrationBuilder.AddForeignKey(
                name: "FK_VideoLike_Account_AccountID1",
                table: "VideoLike",
                column: "AccountID1",
                principalTable: "Account",
                principalColumn: "AccountID");
        }
    }
}
