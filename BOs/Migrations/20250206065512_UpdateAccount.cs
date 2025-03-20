using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BOs.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "Account");

            migrationBuilder.AddColumn<int>(
                name: "AccountID1",
                table: "VideoLike",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RoleID",
                table: "Account",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    RoleID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.RoleID);
                });

            migrationBuilder.InsertData(
                table: "Role",
                columns: new[] { "RoleID", "RoleName" },
                values: new object[,]
                {
                    { 1, "User" },
                    { 2, "SchoolOwner" },
                    { 3, "Admin" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_VideoLike_AccountID1",
                table: "VideoLike",
                column: "AccountID1");

            migrationBuilder.CreateIndex(
                name: "IX_Account_RoleID",
                table: "Account",
                column: "RoleID");

            migrationBuilder.AddForeignKey(
                name: "FK_Account_Role_RoleID",
                table: "Account",
                column: "RoleID",
                principalTable: "Role",
                principalColumn: "RoleID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VideoLike_Account_AccountID1",
                table: "VideoLike",
                column: "AccountID1",
                principalTable: "Account",
                principalColumn: "AccountID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Account_Role_RoleID",
                table: "Account");

            migrationBuilder.DropForeignKey(
                name: "FK_VideoLike_Account_AccountID1",
                table: "VideoLike");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropIndex(
                name: "IX_VideoLike_AccountID1",
                table: "VideoLike");

            migrationBuilder.DropIndex(
                name: "IX_Account_RoleID",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "AccountID1",
                table: "VideoLike");

            migrationBuilder.DropColumn(
                name: "RoleID",
                table: "Account");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Account",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }
    }
}
