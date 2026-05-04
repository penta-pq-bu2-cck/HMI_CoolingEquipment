using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HMI_CoolingEquipment.Migrations
{
    public partial class v1100 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JOBDETAILS",
                columns: table => new
                {
                    JobID = table.Column<string>(maxLength: 50, nullable: false),
                    LotID = table.Column<string>(maxLength: 50, nullable: false),
                    CarrierID = table.Column<string>(maxLength: 50, nullable: false),
                    PortLocation = table.Column<string>(maxLength: 50, nullable: true),
                    LoadingTime = table.Column<DateTime>(type: "DateTime", nullable: true),
                    UnloadingTime = table.Column<DateTime>(type: "DateTime", nullable: true),
                    CarrierStatus = table.Column<string>(maxLength: 50, nullable: true),
                    Type = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JOBDETAILS", x => new { x.JobID, x.LotID, x.CarrierID });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JOBDETAILS");
        }
    }
}
