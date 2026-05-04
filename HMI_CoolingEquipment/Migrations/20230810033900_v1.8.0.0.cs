using Microsoft.EntityFrameworkCore.Migrations;

namespace HMI_CoolingEquipment.Migrations
{
    public partial class v1800 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BinIPAddress",
                table: "BIN",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BinNodeAddress",
                table: "BIN",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BinIPAddress",
                table: "BIN");

            migrationBuilder.DropColumn(
                name: "BinNodeAddress",
                table: "BIN");
        }
    }
}
