using Microsoft.EntityFrameworkCore.Migrations;

namespace HMI_CoolingEquipment.Migrations
{
    public partial class v1201 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "No",
                table: "ALARMHISTORY",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "No",
                table: "ALARMCURRENT",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ALARMHISTORY",
                table: "ALARMHISTORY",
                column: "No");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ALARMCURRENT",
                table: "ALARMCURRENT",
                column: "No");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ALARMHISTORY",
                table: "ALARMHISTORY");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ALARMCURRENT",
                table: "ALARMCURRENT");

            migrationBuilder.DropColumn(
                name: "No",
                table: "ALARMHISTORY");

            migrationBuilder.DropColumn(
                name: "No",
                table: "ALARMCURRENT");
        }
    }
}
