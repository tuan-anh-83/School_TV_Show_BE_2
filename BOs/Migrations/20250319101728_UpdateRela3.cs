using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BOs.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRela3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Program_Schedule_ScheduleID",
                table: "Program");

            migrationBuilder.DropIndex(
                name: "IX_Program_ScheduleID",
                table: "Program");

            migrationBuilder.DropColumn(
                name: "ScheduleID",
                table: "Program");

            migrationBuilder.AddColumn<int>(
                name: "ProgramID",
                table: "Schedule",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Schedule_ProgramID",
                table: "Schedule",
                column: "ProgramID");

            migrationBuilder.AddForeignKey(
                name: "FK_Schedule_Program_ProgramID",
                table: "Schedule",
                column: "ProgramID",
                principalTable: "Program",
                principalColumn: "ProgramID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Schedule_Program_ProgramID",
                table: "Schedule");

            migrationBuilder.DropIndex(
                name: "IX_Schedule_ProgramID",
                table: "Schedule");

            migrationBuilder.DropColumn(
                name: "ProgramID",
                table: "Schedule");

            migrationBuilder.AddColumn<int>(
                name: "ScheduleID",
                table: "Program",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Program_ScheduleID",
                table: "Program",
                column: "ScheduleID");

            migrationBuilder.AddForeignKey(
                name: "FK_Program_Schedule_ScheduleID",
                table: "Program",
                column: "ScheduleID",
                principalTable: "Schedule",
                principalColumn: "ScheduleID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
