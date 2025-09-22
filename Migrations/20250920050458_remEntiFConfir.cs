using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AvyyanBackend.Migrations
{
    /// <inheritdoc />
    public partial class remEntiFConfir : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "RollConfirmations");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "RollConfirmations");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "RollConfirmations");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "RollConfirmations");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "RollConfirmations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "RollConfirmations",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "RollConfirmations",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "RollConfirmations",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "RollConfirmations",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "RollConfirmations",
                type: "text",
                nullable: true);
        }
    }
}
