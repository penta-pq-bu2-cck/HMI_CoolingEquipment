using Microsoft.EntityFrameworkCore.Migrations;

namespace HMI_CoolingEquipment.Migrations
{
    public partial class v1900 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "BinError",
                table: "BIN",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "BinNodeConnectionStatus",
                table: "BIN",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "BinReadyToLoad",
                table: "BIN",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "BinReadyToUnload",
                table: "BIN",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "BinState",
                table: "BIN",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BinError",
                table: "BIN");

            migrationBuilder.DropColumn(
                name: "BinNodeConnectionStatus",
                table: "BIN");

            migrationBuilder.DropColumn(
                name: "BinReadyToLoad",
                table: "BIN");

            migrationBuilder.DropColumn(
                name: "BinReadyToUnload",
                table: "BIN");

            migrationBuilder.DropColumn(
                name: "BinState",
                table: "BIN");
        }
    }
}
