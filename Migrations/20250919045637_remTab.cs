using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AvyyanBackend.Migrations
{
    /// <inheritdoc />
    public partial class remTab : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductionConfirmations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductionConfirmations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ActualWeight = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    AllotId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    BlendPercent = table.Column<decimal>(type: "numeric(5,2)", precision: 18, scale: 3, nullable: true),
                    Cotton = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 3, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GreyGsm = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 3, nullable: true),
                    GreyWidth = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 3, nullable: true),
                    MachineName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Polyester = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 3, nullable: true),
                    RollNo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Spandex = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 3, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionConfirmations", x => x.Id);
                });
        }
    }
}
