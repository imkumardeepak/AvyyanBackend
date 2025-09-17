using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AvyyanBackend.Migrations
{
    /// <inheritdoc />
    public partial class AllT : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MachineAllocations");

            migrationBuilder.DropTable(
                name: "ProductionAllotments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductionAllotments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ActualQuantity = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    AllotmentId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Composition = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Diameter = table.Column<int>(type: "integer", nullable: false),
                    Efficiency = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    FabricType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Gauge = table.Column<int>(type: "integer", nullable: false),
                    ItemName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    RollPerKg = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    SalesOrderId = table.Column<int>(type: "integer", nullable: false),
                    SalesOrderItemId = table.Column<int>(type: "integer", nullable: false),
                    SlitLine = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    StitchLength = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TotalProductionTime = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    VoucherNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    YarnCount = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionAllotments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MachineAllocations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductionAllotmentId = table.Column<int>(type: "integer", nullable: false),
                    AllocatedWeight = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Constant = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Efficiency = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    EstimatedProductionTime = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Feeders = table.Column<int>(type: "integer", nullable: false),
                    FractionalRollWeight = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    MachineId = table.Column<int>(type: "integer", nullable: false),
                    MachineName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    NumberOfNeedles = table.Column<int>(type: "integer", nullable: false),
                    RPM = table.Column<int>(type: "integer", nullable: false),
                    TotalRolls = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    WholeRolls = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MachineAllocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MachineAllocations_ProductionAllotments_ProductionAllotment~",
                        column: x => x.ProductionAllotmentId,
                        principalTable: "ProductionAllotments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MachineAllocations_ProductionAllotmentId",
                table: "MachineAllocations",
                column: "ProductionAllotmentId");
        }
    }
}
