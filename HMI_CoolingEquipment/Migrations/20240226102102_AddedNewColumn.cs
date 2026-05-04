using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HMI_CoolingEquipment.Migrations
{
    public partial class AddedNewColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LD_EXPECTED_END",
                table: "SETUPJOBHISTORY",
                type: "DateTime",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ST_EXPECTED_END",
                table: "SETUPJOBHISTORY",
                type: "DateTime",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LD_EXPECTED_END",
                table: "SETUPJOBHISTORY");

            migrationBuilder.DropColumn(
                name: "ST_EXPECTED_END",
                table: "SETUPJOBHISTORY");
        }
    }
}
