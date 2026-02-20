using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InformationSystemOfASchoolIducationalPortal.Migrations
{
    /// <inheritdoc />
    public partial class addcolumnjournalIdInJournalEntry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JournalEntri_Journal_JournalId",
                table: "JournalEntri");

            migrationBuilder.AlterColumn<string>(
                name: "JournalId",
                table: "JournalEntri",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_JournalEntri_Journal_JournalId",
                table: "JournalEntri",
                column: "JournalId",
                principalTable: "Journal",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JournalEntri_Journal_JournalId",
                table: "JournalEntri");

            migrationBuilder.AlterColumn<string>(
                name: "JournalId",
                table: "JournalEntri",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddForeignKey(
                name: "FK_JournalEntri_Journal_JournalId",
                table: "JournalEntri",
                column: "JournalId",
                principalTable: "Journal",
                principalColumn: "Id");
        }
    }
}
