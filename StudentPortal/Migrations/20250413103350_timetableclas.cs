using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentPortal.Migrations
{
    /// <inheritdoc />
    public partial class timetableclas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsNoted",
                table: "TimetableConflictReports",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_TimetableConflictReports_StudentId",
                table: "TimetableConflictReports",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_TimetableConflictReports_Students_StudentId",
                table: "TimetableConflictReports",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "StudentId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TimetableConflictReports_Students_StudentId",
                table: "TimetableConflictReports");

            migrationBuilder.DropIndex(
                name: "IX_TimetableConflictReports_StudentId",
                table: "TimetableConflictReports");

            migrationBuilder.DropColumn(
                name: "IsNoted",
                table: "TimetableConflictReports");
        }
    }
}
