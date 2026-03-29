using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InformationSystemOfASchoolIducationalPortal.Migrations
{
    /// <inheritdoc />
    public partial class AddNewColumnInTerm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "QuarterNumber",
                table: "Term",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuarterNumber",
                table: "Term");
        }
    }
}
