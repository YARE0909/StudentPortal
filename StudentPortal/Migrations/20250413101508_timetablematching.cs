using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentPortal.Migrations
{
    /// <inheritdoc />
    public partial class timetablematching : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TimetableConflictReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StudentId = table.Column<int>(type: "INTEGER", nullable: false),
                    PreferredCourse = table.Column<string>(type: "TEXT", nullable: false),
                    PreferredDay = table.Column<string>(type: "TEXT", nullable: false),
                    PreferredTimeSlot = table.Column<string>(type: "TEXT", nullable: false),
                    Remarks = table.Column<string>(type: "TEXT", nullable: false),
                    DateSubmitted = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimetableConflictReports", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TimetableConflictReports");
        }
    }
}
