using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BOs.Migrations
{
    /// <inheritdoc />
    public partial class UpdateNews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryNewsID",
                table: "News",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CategoryNews",
                columns: table => new
                {
                    CategoryNewsID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(100)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryNews", x => x.CategoryNewsID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_News_CategoryNewsID",
                table: "News",
                column: "CategoryNewsID");

            migrationBuilder.AddForeignKey(
                name: "FK_News_CategoryNews_CategoryNewsID",
                table: "News",
                column: "CategoryNewsID",
                principalTable: "CategoryNews",
                principalColumn: "CategoryNewsID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_News_CategoryNews_CategoryNewsID",
                table: "News");

            migrationBuilder.DropTable(
                name: "CategoryNews");

            migrationBuilder.DropIndex(
                name: "IX_News_CategoryNewsID",
                table: "News");

            migrationBuilder.DropColumn(
                name: "CategoryNewsID",
                table: "News");
        }
    }
}
