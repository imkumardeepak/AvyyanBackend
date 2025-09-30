using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AvyyanBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddInspectionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                    DoubleParallelYarn = table.Column<int>(type: "integer", nullable: false),
                    HaidJute = table.Column<int>(type: "integer", nullable: false),
                    ColourFabric = table.Column<int>(type: "integer", nullable: false),
                    PP = table.Column<int>(type: "integer", nullable: false),
                    Holes = table.Column<int>(type: "integer", nullable: false),
                    DropStitch = table.Column<int>(type: "integer", nullable: false),
                    LycraStitch = table.Column<int>(type: "integer", nullable: false),
                    LycraBreak = table.Column<int>(type: "integer", nullable: false),
                    FFD = table.Column<int>(type: "integer", nullable: false),
                    NeedleBroken = table.Column<int>(type: "integer", nullable: false),
                    KnitFly = table.Column<int>(type: "integer", nullable: false),
                    OilSpots = table.Column<int>(type: "integer", nullable: false),
                    OilLines = table.Column<int>(type: "integer", nullable: false),
                    VerticalLines = table.Column<int>(type: "integer", nullable: false),
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
        }
    }
}