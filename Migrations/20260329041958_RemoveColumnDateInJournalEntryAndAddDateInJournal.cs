using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InformationSystemOfASchoolIducationalPortal.Migrations
{
    /// <inheritdoc />
    public partial class RemoveColumnDateInJournalEntryAndAddDateInJournal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                table: "JournalEntry");

            migrationBuilder.RenameColumn(
                name: "CanIEdit",
                table: "Journal",
                newName: "IsLocked");

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "Journal",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                table: "Journal");

            migrationBuilder.RenameColumn(
                name: "IsLocked",
                table: "Journal",
                newName: "CanIEdit");

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "JournalEntry",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
