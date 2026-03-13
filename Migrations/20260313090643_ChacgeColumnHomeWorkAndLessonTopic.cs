using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InformationSystemOfASchoolIducationalPortal.Migrations
{
    /// <inheritdoc />
    public partial class ChacgeColumnHomeWorkAndLessonTopic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_TeacherAssigments_AssigmentId",
                table: "Schedules");

            migrationBuilder.DropIndex(
                name: "IX_Schedules_AssigmentId",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "AssigmentId",
                table: "Schedules");

            migrationBuilder.AlterColumn<string>(
                name: "LessonTopic",
                table: "Journal",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "HomeWork",
                table: "Journal",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_TeachinsAssignmentId",
                table: "Schedules",
                column: "TeachinsAssignmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_TeacherAssigments_TeachinsAssignmentId",
                table: "Schedules",
                column: "TeachinsAssignmentId",
                principalTable: "TeacherAssigments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_TeacherAssigments_TeachinsAssignmentId",
                table: "Schedules");

            migrationBuilder.DropIndex(
                name: "IX_Schedules_TeachinsAssignmentId",
                table: "Schedules");

            migrationBuilder.AddColumn<string>(
                name: "AssigmentId",
                table: "Schedules",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LessonTopic",
                table: "Journal",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "HomeWork",
                table: "Journal",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_AssigmentId",
                table: "Schedules",
                column: "AssigmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_TeacherAssigments_AssigmentId",
                table: "Schedules",
                column: "AssigmentId",
                principalTable: "TeacherAssigments",
                principalColumn: "Id");
        }
    }
}
