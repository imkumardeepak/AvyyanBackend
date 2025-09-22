//using System;
//using Microsoft.EntityFrameworkCore.Migrations;
//using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

//#nullable disable

//namespace AvyyanBackend.Migrations
//{
//    /// <inheritdoc />
//    public partial class MachineAllocationDTOtABLE : Migration
//    {
//        /// <inheritdoc />
//        protected override void Up(MigrationBuilder migrationBuilder)
//        {
//            migrationBuilder.CreateTable(
//                name: "ProductionAllotments",
//                columns: table => new
//                {
//                    Id = table.Column<int>(type: "integer", nullable: false)
//                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
//                    AllotmentId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
//                    VoucherNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
//                    ItemName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
//                    SalesOrderId = table.Column<int>(type: "integer", nullable: false),
//                    SalesOrderItemId = table.Column<int>(type: "integer", nullable: false),
//                    ActualQuantity = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
//                    YarnCount = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
//                    Diameter = table.Column<int>(type: "integer", nullable: false),
//                    Gauge = table.Column<int>(type: "integer", nullable: false),
//                    FabricType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
//                    SlitLine = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
//                    StitchLength = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
//                    Efficiency = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
//                    Composition = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
//                    TotalProductionTime = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
//                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_ProductionAllotments", x => x.Id);
//                });

//            migrationBuilder.CreateTable(
//                name: "MachineAllocations",
//                columns: table => new
//                {
//                    Id = table.Column<int>(type: "integer", nullable: false)
//                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
//                    ProductionAllotmentId = table.Column<int>(type: "integer", nullable: false),
//                    MachineName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
//                    MachineId = table.Column<int>(type: "integer", nullable: false),
//                    NumberOfNeedles = table.Column<int>(type: "integer", nullable: false),
//                    Feeders = table.Column<int>(type: "integer", nullable: false),
//                    RPM = table.Column<int>(type: "integer", nullable: false),
//                    RollPerKg = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
//                    TotalLoadWeight = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
//                    TotalRolls = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
//                    RollBreakdown = table.Column<string>(type: "text", nullable: false),
//                    EstimatedProductionTime = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_MachineAllocations", x => x.Id);
//                    table.ForeignKey(
//                        name: "FK_MachineAllocations_ProductionAllotments_ProductionAllotment~",
//                        column: x => x.ProductionAllotmentId,
//                        principalTable: "ProductionAllotments",
//                        principalColumn: "Id",
//                        onDelete: ReferentialAction.Cascade);
//                });

//            migrationBuilder.CreateIndex(
//                name: "IX_MachineAllocations_ProductionAllotmentId",
//                table: "MachineAllocations",
//                column: "ProductionAllotmentId");
//        }

//        /// <inheritdoc />
//        protected override void Down(MigrationBuilder migrationBuilder)
//        {
//            migrationBuilder.DropTable(
//                name: "MachineAllocations");

//            migrationBuilder.DropTable(
//                name: "ProductionAllotments");
//        }
//    }
//}
