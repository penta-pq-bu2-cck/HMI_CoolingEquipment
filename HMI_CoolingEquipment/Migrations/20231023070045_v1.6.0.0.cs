using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HMI_CoolingEquipment.Migrations
{
    public partial class v1600 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EVENTLOG",
                columns: table => new
                {
                    No = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateTime = table.Column<DateTime>(type: "DateTime", nullable: false),
                    EventType = table.Column<string>(nullable: true),
                    EventPage = table.Column<string>(nullable: true),
                    JobID = table.Column<string>(nullable: true),
                    LotID = table.Column<string>(nullable: true),
                    CarrierID = table.Column<string>(nullable: true),
                    LoadPortID = table.Column<string>(nullable: true),
                    UserID = table.Column<string>(nullable: true),
                    EventMessage = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EVENTLOG", x => x.No);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EVENTLOG");
        }
    }
}
