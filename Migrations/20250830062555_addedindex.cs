﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AvyyanBackend.Migrations
{
    /// <inheritdoc />
    public partial class addedindex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_SalesOrders_ProcessFlag",
                table: "SalesOrders",
                column: "ProcessFlag");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SalesOrders_ProcessFlag",
                table: "SalesOrders");
        }
    }
}
