using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AvyyanBackend.Migrations
{
    /// <inheritdoc />
    public partial class RemoveEfficiencyFromMachineManager : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Efficiency",
                table: "MachineManagers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Efficiency",
                table: "MachineManagers",
                type: "numeric(18,5)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
