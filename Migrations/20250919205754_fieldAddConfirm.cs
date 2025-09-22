using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AvyyanBackend.Migrations
{
    /// <inheritdoc />
    public partial class fieldAddConfirm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
        

            migrationBuilder.RenameColumn(
                name: "ActualWeight",
                table: "RollConfirmations",
                newName: "RollPerKg");

            migrationBuilder.AlterColumn<decimal>(
                name: "Spandex",
                table: "RollConfirmations",
                type: "numeric(5,2)",
                precision: 18,
                scale: 3,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 3,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Polyester",
                table: "RollConfirmations",
                type: "numeric(5,2)",
                precision: 18,
                scale: 3,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 3,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "GreyWidth",
                table: "RollConfirmations",
                type: "numeric(18,2)",
                precision: 18,
                scale: 3,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 3,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "GreyGsm",
                table: "RollConfirmations",
                type: "numeric(18,2)",
                precision: 18,
                scale: 3,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 3,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Cotton",
                table: "RollConfirmations",
                type: "numeric(5,2)",
                precision: 18,
                scale: 3,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 3,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "BlendPercent",
                table: "RollConfirmations",
                type: "numeric(5,2)",
                precision: 18,
                scale: 3,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric(5,2)",
                oldPrecision: 18,
                oldScale: 3,
                oldNullable: true);

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

            migrationBuilder.CreateTable(
                name: "Inspections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AllotId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    MachineName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    RollNo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ThinPlaces = table.Column<int>(type: "integer", nullable: false),
                    ThickPlaces = table.Column<int>(type: "integer", nullable: false),
                    ThinLines = table.Column<int>(type: "integer", nullable: false),
                    ThickLines = table.Column<int>(type: "integer", nullable: false),
                    DoubleYarn = table.Column<int>(type: "integer", nullable: false),
                    HairJute = table.Column<int>(type: "integer", nullable: false),
                    ColourFiber = table.Column<int>(type: "integer", nullable: false),
                    Hole = table.Column<int>(type: "integer", nullable: false),
                    DropStitch = table.Column<int>(type: "integer", nullable: false),
                    LycraStitch = table.Column<int>(type: "integer", nullable: false),
                    LycraBreak = table.Column<int>(type: "integer", nullable: false),
                    FFD = table.Column<int>(type: "integer", nullable: false),
                    NeedleBroken = table.Column<int>(type: "integer", nullable: false),
                    KnitFly = table.Column<int>(type: "integer", nullable: false),
                    OilSpots = table.Column<int>(type: "integer", nullable: false),
                    OilLines = table.Column<int>(type: "integer", nullable: false),
                    Grade = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    TotalFaults = table.Column<int>(type: "integer", nullable: false),
                    Remarks = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inspections", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RollConfirmations_AllotId_MachineName_RollNo",
                table: "RollConfirmations",
                columns: new[] { "AllotId", "MachineName", "RollNo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Inspections_AllotId_MachineName_RollNo",
                table: "Inspections",
                columns: new[] { "AllotId", "MachineName", "RollNo" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Inspections");

            migrationBuilder.DropIndex(
                name: "IX_RollConfirmations_AllotId_MachineName_RollNo",
                table: "RollConfirmations");

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

            migrationBuilder.RenameColumn(
                name: "RollPerKg",
                table: "RollConfirmations",
                newName: "ActualWeight");

        

            migrationBuilder.AlterColumn<decimal>(
                name: "Spandex",
                table: "RollConfirmations",
                type: "numeric(18,2)",
                precision: 18,
                scale: 3,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(5,2)",
                oldPrecision: 18,
                oldScale: 3);

            migrationBuilder.AlterColumn<decimal>(
                name: "Polyester",
                table: "RollConfirmations",
                type: "numeric(18,2)",
                precision: 18,
                scale: 3,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(5,2)",
                oldPrecision: 18,
                oldScale: 3);

            migrationBuilder.AlterColumn<decimal>(
                name: "GreyWidth",
                table: "RollConfirmations",
                type: "numeric(18,2)",
                precision: 18,
                scale: 3,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 3);

            migrationBuilder.AlterColumn<decimal>(
                name: "GreyGsm",
                table: "RollConfirmations",
                type: "numeric(18,2)",
                precision: 18,
                scale: 3,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 3);

            migrationBuilder.AlterColumn<decimal>(
                name: "Cotton",
                table: "RollConfirmations",
                type: "numeric(18,2)",
                precision: 18,
                scale: 3,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(5,2)",
                oldPrecision: 18,
                oldScale: 3);

            migrationBuilder.AlterColumn<decimal>(
                name: "BlendPercent",
                table: "RollConfirmations",
                type: "numeric(5,2)",
                precision: 18,
                scale: 3,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(5,2)",
                oldPrecision: 18,
                oldScale: 3);
        }
    }
}
