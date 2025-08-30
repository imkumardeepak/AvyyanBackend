using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AvyyanBackend.Migrations
{
    /// <inheritdoc />
    public partial class sales : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SalesOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VchType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    SalesDate = table.Column<DateTime>(type: "timestamp without time zone", maxLength: 20, nullable: false),
                    Guid = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    GstRegistrationType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    StateName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PartyName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    PartyLedgerName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    VoucherNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Reference = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CompanyAddress = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    BuyerAddress = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    OrderTerms = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    LedgerEntries = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesOrders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SalesOrderItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SalesOrderId = table.Column<int>(type: "integer", nullable: false),
                    StockItemName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Rate = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Amount = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ActualQty = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    BilledQty = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Descriptions = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    BatchName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    OrderNo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    OrderDueDate = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesOrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalesOrderItems_SalesOrders_SalesOrderId",
                        column: x => x.SalesOrderId,
                        principalTable: "SalesOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrderItems_SalesOrderId",
                table: "SalesOrderItems",
                column: "SalesOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrderItems_StockItemName",
                table: "SalesOrderItems",
                column: "StockItemName");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrders_PartyName",
                table: "SalesOrders",
                column: "PartyName");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrders_SalesDate",
                table: "SalesOrders",
                column: "SalesDate");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrders_VoucherNumber",
                table: "SalesOrders",
                column: "VoucherNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SalesOrderItems");

            migrationBuilder.DropTable(
                name: "SalesOrders");
        }
    }
}
