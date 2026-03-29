using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InformationSystemOfASchoolIducationalPortal.Migrations
{
    /// <inheritdoc />
    public partial class AddAcademicYearAddTermAddStudentHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AcademicYear",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    StartDateYear = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndDateYear = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcademicYear", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Term",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    DateStartTerm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateEndTerm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AcademicYearId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Term", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Term_AcademicYear_AcademicYearId",
                        column: x => x.AcademicYearId,
                        principalTable: "AcademicYear",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentsHistory",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    StudentId = table.Column<string>(type: "TEXT", nullable: false),
                    ClassId = table.Column<string>(type: "TEXT", nullable: false),
                    AcademicYearId = table.Column<string>(type: "TEXT", nullable: false),
                    TermId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentsHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentsHistory_AcademicYear_AcademicYearId",
                        column: x => x.AcademicYearId,
                        principalTable: "AcademicYear",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentsHistory_Classes_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentsHistory_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentsHistory_Term_TermId",
                        column: x => x.TermId,
                        principalTable: "Term",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentsHistory_AcademicYearId",
                table: "StudentsHistory",
                column: "AcademicYearId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentsHistory_ClassId",
                table: "StudentsHistory",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentsHistory_StudentId",
                table: "StudentsHistory",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentsHistory_TermId",
                table: "StudentsHistory",
                column: "TermId");

            migrationBuilder.CreateIndex(
                name: "IX_Term_AcademicYearId",
                table: "Term",
                column: "AcademicYearId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentsHistory");

            migrationBuilder.DropTable(
                name: "Term");

            migrationBuilder.DropTable(
                name: "AcademicYear");
        }
    }
}
