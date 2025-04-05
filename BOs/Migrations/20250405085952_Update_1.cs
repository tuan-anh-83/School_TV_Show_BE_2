using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BOs.Migrations
{
    /// <inheritdoc />
    public partial class Update_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Account_AccountID",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetail_Package_PackageID",
                table: "OrderDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_Share_Account_AccountID",
                table: "Share");

            migrationBuilder.DropForeignKey(
                name: "FK_VideoHistory_Program_ProgramID",
                table: "VideoHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_VideoLike_Account_AccountID",
                table: "VideoLike");

            migrationBuilder.DropIndex(
                name: "IX_VideoLike_AccountID",
                table: "VideoLike");

            migrationBuilder.DropIndex(
                name: "IX_Share_AccountID",
                table: "Share");

            migrationBuilder.DropIndex(
                name: "IX_Comment_AccountID",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "Mode",
                table: "Schedule");

            migrationBuilder.AddColumn<int>(
                name: "AccountID",
                table: "VideoView",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "ProgramID",
                table: "VideoHistory",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "MP4Url",
                table: "VideoHistory",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Duration",
                table: "VideoHistory",
                type: "float",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Schedule",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Pending",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldDefaultValue: "Active");

            migrationBuilder.AlterColumn<string>(
                name: "CloudflareStreamId",
                table: "Program",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Package",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "True",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldDefaultValue: "Active");

            migrationBuilder.AddColumn<int>(
                name: "PackageID1",
                table: "OrderDetail",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Comment",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetail_PackageID1",
                table: "OrderDetail",
                column: "PackageID1");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetail_Package_PackageID",
                table: "OrderDetail",
                column: "PackageID",
                principalTable: "Package",
                principalColumn: "PackageID");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetail_Package_PackageID1",
                table: "OrderDetail",
                column: "PackageID1",
                principalTable: "Package",
                principalColumn: "PackageID");

            migrationBuilder.AddForeignKey(
                name: "FK_VideoHistory_Program_ProgramID",
                table: "VideoHistory",
                column: "ProgramID",
                principalTable: "Program",
                principalColumn: "ProgramID",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetail_Package_PackageID",
                table: "OrderDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetail_Package_PackageID1",
                table: "OrderDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_VideoHistory_Program_ProgramID",
                table: "VideoHistory");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetail_PackageID1",
                table: "OrderDetail");

            migrationBuilder.DropColumn(
                name: "AccountID",
                table: "VideoView");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "VideoHistory");

            migrationBuilder.DropColumn(
                name: "PackageID1",
                table: "OrderDetail");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Comment");

            migrationBuilder.AlterColumn<int>(
                name: "ProgramID",
                table: "VideoHistory",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MP4Url",
                table: "VideoHistory",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Schedule",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Active",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldDefaultValue: "Pending");

            migrationBuilder.AddColumn<string>(
                name: "Mode",
                table: "Schedule",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "Live");

            migrationBuilder.AlterColumn<string>(
                name: "CloudflareStreamId",
                table: "Program",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Package",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Active",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValue: "True");

            migrationBuilder.CreateIndex(
                name: "IX_VideoLike_AccountID",
                table: "VideoLike",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_Share_AccountID",
                table: "Share",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_AccountID",
                table: "Comment",
                column: "AccountID");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Account_AccountID",
                table: "Comment",
                column: "AccountID",
                principalTable: "Account",
                principalColumn: "AccountID");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetail_Package_PackageID",
                table: "OrderDetail",
                column: "PackageID",
                principalTable: "Package",
                principalColumn: "PackageID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Share_Account_AccountID",
                table: "Share",
                column: "AccountID",
                principalTable: "Account",
                principalColumn: "AccountID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VideoHistory_Program_ProgramID",
                table: "VideoHistory",
                column: "ProgramID",
                principalTable: "Program",
                principalColumn: "ProgramID");

            migrationBuilder.AddForeignKey(
                name: "FK_VideoLike_Account_AccountID",
                table: "VideoLike",
                column: "AccountID",
                principalTable: "Account",
                principalColumn: "AccountID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
