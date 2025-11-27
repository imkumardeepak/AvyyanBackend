using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AvyyanBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddCOlINSaleOrderWeb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DispatchThrough",
                table: "SalesOrdersWeb",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsProcess",
                table: "SalesOrdersWeb",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "OrderNo",
                table: "SalesOrdersWeb",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TermsOfDelivery",
                table: "SalesOrdersWeb",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsProcess",
                table: "SalesOrderItemsWeb",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Unit",
                table: "SalesOrderItemsWeb",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DispatchThrough",
                table: "SalesOrdersWeb");

            migrationBuilder.DropColumn(
                name: "IsProcess",
                table: "SalesOrdersWeb");

            migrationBuilder.DropColumn(
                name: "OrderNo",
                table: "SalesOrdersWeb");

            migrationBuilder.DropColumn(
                name: "TermsOfDelivery",
                table: "SalesOrdersWeb");

            migrationBuilder.DropColumn(
                name: "IsProcess",
                table: "SalesOrderItemsWeb");

            migrationBuilder.DropColumn(
                name: "Unit",
                table: "SalesOrderItemsWeb");
        }
    }
}
