using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InformationSystemOfASchoolIducationalPortal.Migrations
{
    /// <inheritdoc />
    public partial class DeletColumnHomeWorkAndLessonTopiAndAddTheesColumnInJournal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HomeWork",
                table: "JournalEntri");

            migrationBuilder.DropColumn(
                name: "LessonTopic",
                table: "JournalEntri");

            migrationBuilder.AddColumn<string>(
                name: "HomeWork",
                table: "Journal",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LessonTopic",
                table: "Journal",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HomeWork",
                table: "Journal");

            migrationBuilder.DropColumn(
                name: "LessonTopic",
                table: "Journal");

            migrationBuilder.AddColumn<string>(
                name: "HomeWork",
                table: "JournalEntri",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LessonTopic",
                table: "JournalEntri",
                type: "TEXT",
                nullable: true);
        }
    }
}
