using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InformationSystemOfASchoolIducationalPortal.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnsInJournal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AcademicYearId",
                table: "Journal",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TermId",
                table: "Journal",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Journal_AcademicYearId",
                table: "Journal",
                column: "AcademicYearId");

            migrationBuilder.CreateIndex(
                name: "IX_Journal_TermId",
                table: "Journal",
                column: "TermId");

            migrationBuilder.AddForeignKey(
                name: "FK_Journal_AcademicYear_AcademicYearId",
                table: "Journal",
                column: "AcademicYearId",
                principalTable: "AcademicYear",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Journal_Term_TermId",
                table: "Journal",
                column: "TermId",
                principalTable: "Term",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Journal_AcademicYear_AcademicYearId",
                table: "Journal");

            migrationBuilder.DropForeignKey(
                name: "FK_Journal_Term_TermId",
                table: "Journal");

            migrationBuilder.DropIndex(
                name: "IX_Journal_AcademicYearId",
                table: "Journal");

            migrationBuilder.DropIndex(
                name: "IX_Journal_TermId",
                table: "Journal");

            migrationBuilder.DropColumn(
                name: "AcademicYearId",
                table: "Journal");

            migrationBuilder.DropColumn(
                name: "TermId",
                table: "Journal");
        }
    }
}
