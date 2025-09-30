using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AvyyanBackend.Migrations
{
    /// <inheritdoc />
    public partial class entityFTAlotment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "ShiftMasters");

            migrationBuilder.AddColumn<string>(
                name: "TapeColor",
                table: "ProductionAllotments",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "TubeWeight",
                table: "ProductionAllotments",
                type: "numeric(18,3)",
                precision: 18,
                scale: 3,
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TapeColor",
                table: "ProductionAllotments");

            migrationBuilder.DropColumn(
                name: "TubeWeight",
                table: "ProductionAllotments");

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "ShiftMasters",
                type: "integer",
                nullable: true);
        }
    }
}
