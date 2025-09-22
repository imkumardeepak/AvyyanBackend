//using Microsoft.EntityFrameworkCore.Migrations;

//#nullable disable

//namespace AvyyanBackend.Migrations
//{
//    /// <inheritdoc />
//    public partial class addEntityProAllot : Migration
//    {
//        /// <inheritdoc />
//        protected override void Up(MigrationBuilder migrationBuilder)
//        {
//            migrationBuilder.AddColumn<string>(
//                name: "ColourCode",
//                table: "ProductionAllotments",
//                type: "character varying(50)",
//                maxLength: 50,
//                nullable: false,
//                defaultValue: "");

//            migrationBuilder.AddColumn<string>(
//                name: "Counter",
//                table: "ProductionAllotments",
//                type: "character varying(50)",
//                maxLength: 50,
//                nullable: false,
//                defaultValue: "");

//            migrationBuilder.AddColumn<string>(
//                name: "PartyName",
//                table: "ProductionAllotments",
//                type: "character varying(200)",
//                maxLength: 200,
//                nullable: false,
//                defaultValue: "");

//            migrationBuilder.AddColumn<decimal>(
//                name: "ReqFinishGsm",
//                table: "ProductionAllotments",
//                type: "numeric(18,2)",
//                precision: 18,
//                scale: 3,
//                nullable: true);

//            migrationBuilder.AddColumn<decimal>(
//                name: "ReqFinishWidth",
//                table: "ProductionAllotments",
//                type: "numeric(18,2)",
//                precision: 18,
//                scale: 3,
//                nullable: true);

//            migrationBuilder.AddColumn<decimal>(
//                name: "ReqGreyGsm",
//                table: "ProductionAllotments",
//                type: "numeric(18,2)",
//                precision: 18,
//                scale: 3,
//                nullable: true);

//            migrationBuilder.AddColumn<decimal>(
//                name: "ReqGreyWidth",
//                table: "ProductionAllotments",
//                type: "numeric(18,2)",
//                precision: 18,
//                scale: 3,
//                nullable: true);

//            migrationBuilder.AddColumn<string>(
//                name: "YarnLotNo",
//                table: "ProductionAllotments",
//                type: "character varying(50)",
//                maxLength: 50,
//                nullable: false,
//                defaultValue: "");
//        }

//        /// <inheritdoc />
//        protected override void Down(MigrationBuilder migrationBuilder)
//        {
//            migrationBuilder.DropColumn(
//                name: "ColourCode",
//                table: "ProductionAllotments");

//            migrationBuilder.DropColumn(
//                name: "Counter",
//                table: "ProductionAllotments");

//            migrationBuilder.DropColumn(
//                name: "PartyName",
//                table: "ProductionAllotments");

//            migrationBuilder.DropColumn(
//                name: "ReqFinishGsm",
//                table: "ProductionAllotments");

//            migrationBuilder.DropColumn(
//                name: "ReqFinishWidth",
//                table: "ProductionAllotments");

//            migrationBuilder.DropColumn(
//                name: "ReqGreyGsm",
//                table: "ProductionAllotments");

//            migrationBuilder.DropColumn(
//                name: "ReqGreyWidth",
//                table: "ProductionAllotments");

//            migrationBuilder.DropColumn(
//                name: "YarnLotNo",
//                table: "ProductionAllotments");
//        }
//    }
//}
