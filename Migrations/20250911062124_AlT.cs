using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AvyyanBackend.Migrations
{
    /// <inheritdoc />
    public partial class AlT : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MachineAllocation_ProductionAllotment_ProductionAllotmentId",
                table: "MachineAllocation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductionAllotment",
                table: "ProductionAllotment");

            migrationBuilder.DropIndex(
                name: "IX_ProductionAllotment_AllotmentId",
                table: "ProductionAllotment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MachineAllocation",
                table: "MachineAllocation");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "ProductionAllotment");

            migrationBuilder.DropColumn(
                name: "RollBreakdown",
                table: "MachineAllocation");

            migrationBuilder.RenameTable(
                name: "ProductionAllotment",
                newName: "ProductionAllotments");

            migrationBuilder.RenameTable(
                name: "MachineAllocation",
                newName: "MachineAllocations");

            migrationBuilder.RenameColumn(
                name: "TotalLoadWeight",
                table: "MachineAllocations",
                newName: "FractionalRollWeight");

            migrationBuilder.RenameColumn(
                name: "RollPerKg",
                table: "MachineAllocations",
                newName: "Constant");

            migrationBuilder.RenameIndex(
                name: "IX_MachineAllocation_ProductionAllotmentId",
                table: "MachineAllocations",
                newName: "IX_MachineAllocations_ProductionAllotmentId");

            migrationBuilder.AddColumn<decimal>(
                name: "RollPerKg",
                table: "ProductionAllotments",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AllocatedWeight",
                table: "MachineAllocations",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Efficiency",
                table: "MachineAllocations",
                type: "numeric(5,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "MachineId",
                table: "MachineAllocations",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WholeRolls",
                table: "MachineAllocations",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductionAllotments",
                table: "ProductionAllotments",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MachineAllocations",
                table: "MachineAllocations",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MachineAllocations_ProductionAllotments_ProductionAllotment~",
                table: "MachineAllocations",
                column: "ProductionAllotmentId",
                principalTable: "ProductionAllotments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MachineAllocations_ProductionAllotments_ProductionAllotment~",
                table: "MachineAllocations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductionAllotments",
                table: "ProductionAllotments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MachineAllocations",
                table: "MachineAllocations");

            migrationBuilder.DropColumn(
                name: "RollPerKg",
                table: "ProductionAllotments");

            migrationBuilder.DropColumn(
                name: "AllocatedWeight",
                table: "MachineAllocations");

            migrationBuilder.DropColumn(
                name: "Efficiency",
                table: "MachineAllocations");

            migrationBuilder.DropColumn(
                name: "MachineId",
                table: "MachineAllocations");

            migrationBuilder.DropColumn(
                name: "WholeRolls",
                table: "MachineAllocations");

            migrationBuilder.RenameTable(
                name: "ProductionAllotments",
                newName: "ProductionAllotment");

            migrationBuilder.RenameTable(
                name: "MachineAllocations",
                newName: "MachineAllocation");

            migrationBuilder.RenameColumn(
                name: "FractionalRollWeight",
                table: "MachineAllocation",
                newName: "TotalLoadWeight");

            migrationBuilder.RenameColumn(
                name: "Constant",
                table: "MachineAllocation",
                newName: "RollPerKg");

            migrationBuilder.RenameIndex(
                name: "IX_MachineAllocations_ProductionAllotmentId",
                table: "MachineAllocation",
                newName: "IX_MachineAllocation_ProductionAllotmentId");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "ProductionAllotment",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RollBreakdown",
                table: "MachineAllocation",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductionAllotment",
                table: "ProductionAllotment",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MachineAllocation",
                table: "MachineAllocation",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionAllotment_AllotmentId",
                table: "ProductionAllotment",
                column: "AllotmentId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_MachineAllocation_ProductionAllotment_ProductionAllotmentId",
                table: "MachineAllocation",
                column: "ProductionAllotmentId",
                principalTable: "ProductionAllotment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
