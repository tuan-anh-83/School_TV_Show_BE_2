using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BOs.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDB2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_News_SchoolChannel_SchoolChannelID",
                table: "News");

            migrationBuilder.DropForeignKey(
                name: "FK_NewsPicture_News_NewsID",
                table: "NewsPicture");

            migrationBuilder.DropForeignKey(
                name: "FK_Program_SchoolChannel_ChannelID",
                table: "Program");

            migrationBuilder.DropForeignKey(
                name: "FK_Share_Account_AccountID",
                table: "Share");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NewsPicture",
                table: "NewsPicture");

            migrationBuilder.DropColumn(
                name: "Content",
                table: "Share");

            migrationBuilder.RenameTable(
                name: "NewsPicture",
                newName: "NewsPictures");

            migrationBuilder.RenameColumn(
                name: "ChannelID",
                table: "Program",
                newName: "SchoolChannelID");

            migrationBuilder.RenameIndex(
                name: "IX_Program_ChannelID",
                table: "Program",
                newName: "IX_Program_SchoolChannelID");

            migrationBuilder.RenameIndex(
                name: "IX_NewsPicture_NewsID",
                table: "NewsPictures",
                newName: "IX_NewsPictures_NewsID");

            migrationBuilder.AlterColumn<bool>(
                name: "Status",
                table: "VideoHistory",
                type: "bit",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Package",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "True",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValue: "Active");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "News",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "News",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<bool>(
                name: "FollowerMode",
                table: "News",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Account",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Account",
                type: "nvarchar(800)",
                maxLength: 800,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<string>(
                name: "ExternalProvider",
                table: "Account",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExternalProviderKey",
                table: "Account",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                table: "NewsPictures",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "ContentType",
                table: "NewsPictures",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddPrimaryKey(
                name: "PK_NewsPictures",
                table: "NewsPictures",
                column: "PictureID");

            migrationBuilder.CreateTable(
                name: "Follow",
                columns: table => new
                {
                    AccountID = table.Column<int>(type: "int", nullable: false),
                    SchoolChannelID = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false, defaultValue: "Followed"),
                    FollowedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Follow", x => new { x.AccountID, x.SchoolChannelID });
                    table.ForeignKey(
                        name: "FK_Follow_Account_AccountID",
                        column: x => x.AccountID,
                        principalTable: "Account",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Follow_SchoolChannel_SchoolChannelID",
                        column: x => x.SchoolChannelID,
                        principalTable: "SchoolChannel",
                        principalColumn: "SchoolChannelID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PasswordResetToken",
                columns: table => new
                {
                    PasswordResetTokenID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountID = table.Column<int>(type: "int", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Expiration = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordResetToken", x => x.PasswordResetTokenID);
                    table.ForeignKey(
                        name: "FK_PasswordResetToken_Account_AccountID",
                        column: x => x.AccountID,
                        principalTable: "Account",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Follow_SchoolChannelID",
                table: "Follow",
                column: "SchoolChannelID");

            migrationBuilder.CreateIndex(
                name: "IX_PasswordResetToken_AccountID",
                table: "PasswordResetToken",
                column: "AccountID");

            migrationBuilder.AddForeignKey(
                name: "FK_News_SchoolChannel_SchoolChannelID",
                table: "News",
                column: "SchoolChannelID",
                principalTable: "SchoolChannel",
                principalColumn: "SchoolChannelID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_NewsPictures_News_NewsID",
                table: "NewsPictures",
                column: "NewsID",
                principalTable: "News",
                principalColumn: "NewsID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Program_SchoolChannel_SchoolChannelID",
                table: "Program",
                column: "SchoolChannelID",
                principalTable: "SchoolChannel",
                principalColumn: "SchoolChannelID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Share_Account_AccountID",
                table: "Share",
                column: "AccountID",
                principalTable: "Account",
                principalColumn: "AccountID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_News_SchoolChannel_SchoolChannelID",
                table: "News");

            migrationBuilder.DropForeignKey(
                name: "FK_NewsPictures_News_NewsID",
                table: "NewsPictures");

            migrationBuilder.DropForeignKey(
                name: "FK_Program_SchoolChannel_SchoolChannelID",
                table: "Program");

            migrationBuilder.DropForeignKey(
                name: "FK_Share_Account_AccountID",
                table: "Share");

            migrationBuilder.DropTable(
                name: "Follow");

            migrationBuilder.DropTable(
                name: "PasswordResetToken");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NewsPictures",
                table: "NewsPictures");

            migrationBuilder.DropColumn(
                name: "FollowerMode",
                table: "News");

            migrationBuilder.DropColumn(
                name: "ExternalProvider",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "ExternalProviderKey",
                table: "Account");

            migrationBuilder.RenameTable(
                name: "NewsPictures",
                newName: "NewsPicture");

            migrationBuilder.RenameColumn(
                name: "SchoolChannelID",
                table: "Program",
                newName: "ChannelID");

            migrationBuilder.RenameIndex(
                name: "IX_Program_SchoolChannelID",
                table: "Program",
                newName: "IX_Program_ChannelID");

            migrationBuilder.RenameIndex(
                name: "IX_NewsPictures_NewsID",
                table: "NewsPicture",
                newName: "IX_NewsPicture_NewsID");

            migrationBuilder.AlterColumn<bool>(
                name: "Status",
                table: "VideoHistory",
                type: "bit",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "Share",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Package",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "Active",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValue: "True");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "News",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "News",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<bool>(
                name: "Status",
                table: "Account",
                type: "bit",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Account",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(800)",
                oldMaxLength: 800);

            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                table: "NewsPicture",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ContentType",
                table: "NewsPicture",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NewsPicture",
                table: "NewsPicture",
                column: "PictureID");

            migrationBuilder.AddForeignKey(
                name: "FK_News_SchoolChannel_SchoolChannelID",
                table: "News",
                column: "SchoolChannelID",
                principalTable: "SchoolChannel",
                principalColumn: "SchoolChannelID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NewsPicture_News_NewsID",
                table: "NewsPicture",
                column: "NewsID",
                principalTable: "News",
                principalColumn: "NewsID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Program_SchoolChannel_ChannelID",
                table: "Program",
                column: "ChannelID",
                principalTable: "SchoolChannel",
                principalColumn: "SchoolChannelID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Share_Account_AccountID",
                table: "Share",
                column: "AccountID",
                principalTable: "Account",
                principalColumn: "AccountID");
        }
    }
}
