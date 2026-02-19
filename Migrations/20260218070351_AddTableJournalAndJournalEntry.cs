using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InformationSystemOfASchoolIducationalPortal.Migrations
{
    /// <inheritdoc />
    public partial class AddTableJournalAndJournalEntry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Journal",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    TeacherId = table.Column<string>(type: "TEXT", nullable: false),
                    SubjectId = table.Column<string>(type: "TEXT", nullable: false),
                    ClassId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Journal", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Journal_Classes_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Journal_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Journal_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JournalEntri",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    StudentId = table.Column<string>(type: "TEXT", nullable: false),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Grade = table.Column<string>(type: "TEXT", nullable: true),
                    IsPresent = table.Column<bool>(type: "INTEGER", nullable: false),
                    JournalId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JournalEntri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JournalEntri_Journal_JournalId",
                        column: x => x.JournalId,
                        principalTable: "Journal",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_JournalEntri_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Journal_ClassId",
                table: "Journal",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_Journal_SubjectId",
                table: "Journal",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Journal_TeacherId",
                table: "Journal",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntri_JournalId",
                table: "JournalEntri",
                column: "JournalId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntri_StudentId",
                table: "JournalEntri",
                column: "StudentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JournalEntri");

            migrationBuilder.DropTable(
                name: "Journal");
        }
    }
}
