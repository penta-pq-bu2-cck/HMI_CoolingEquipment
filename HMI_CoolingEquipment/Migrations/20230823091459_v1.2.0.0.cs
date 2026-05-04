using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HMI_CoolingEquipment.Migrations
{
    public partial class v1200 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ALARMCURRENT",
                columns: table => new
                {
                    AlarmDateTime = table.Column<DateTime>(type: "DateTime", nullable: false),
                    AlarmType = table.Column<int>(maxLength: 50, nullable: false),
                    AlarmCode = table.Column<int>(nullable: false),
                    AlarmModule = table.Column<string>(nullable: true),
                    AlarmMessage = table.Column<string>(nullable: true),
                    AlarmAction = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "ALARMHISTORY",
                columns: table => new
                {
                    AlarmDateTime = table.Column<DateTime>(type: "DateTime", nullable: false),
                    AlarmType = table.Column<int>(maxLength: 50, nullable: false),
                    AlarmCode = table.Column<int>(nullable: false),
                    AlarmModule = table.Column<string>(nullable: true),
                    AlarmMessage = table.Column<string>(nullable: true),
                    AlarmAction = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ALARMCURRENT");

            migrationBuilder.DropTable(
                name: "ALARMHISTORY");
        }
    }
}
