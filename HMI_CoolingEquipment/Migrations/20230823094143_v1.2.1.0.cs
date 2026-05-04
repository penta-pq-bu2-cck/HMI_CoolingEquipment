using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HMI_CoolingEquipment.Migrations
{
    public partial class v1210 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AlarmClearedDateTime",
                table: "ALARMHISTORY",
                type: "DateTime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AlarmClearedDateTime",
                table: "ALARMHISTORY");
        }
    }
}
