using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AvyyanBackend.Migrations
{
    /// <inheritdoc />
    public partial class addTConfirmation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductionConfirmations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AllotId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    MachineName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    RollNo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    GreyGsm = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 3, nullable: true),
                    GreyWidth = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 3, nullable: true),
                    ActualWeight = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    BlendPercent = table.Column<decimal>(type: "numeric(5,2)", precision: 18, scale: 3, nullable: true),
                    Cotton = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 3, nullable: true),
                    Polyester = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 3, nullable: true),
                    Spandex = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 3, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionConfirmations", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductionConfirmations");
        }
    }
}
