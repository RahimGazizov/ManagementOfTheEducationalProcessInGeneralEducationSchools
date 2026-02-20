using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InformationSystemOfASchoolIducationalPortal.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnLessonTopicAndColumnHomeWorkInJournalEntry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HomeWork",
                table: "JournalEntri");

            migrationBuilder.DropColumn(
                name: "LessonTopic",
                table: "JournalEntri");
        }
    }
}
