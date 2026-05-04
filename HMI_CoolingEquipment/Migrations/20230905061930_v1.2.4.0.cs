using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HMI_CoolingEquipment.Migrations
{
    public partial class v1240 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JOBDETAILSHISTORY",
                columns: table => new
                {
                    No = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobID = table.Column<string>(maxLength: 50, nullable: false),
                    LotNo = table.Column<string>(maxLength: 50, nullable: false),
                    CarrierID = table.Column<string>(maxLength: 50, nullable: false),
                    PortLocation = table.Column<string>(maxLength: 50, nullable: true),
                    LoadingTime = table.Column<DateTime>(type: "DateTime", nullable: true),
                    UnloadingTime = table.Column<DateTime>(type: "DateTime", nullable: true),
                    CarrierStatus = table.Column<int>(nullable: false),
                    Type = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JOBDETAILSHISTORY", x => x.No);
                });

            migrationBuilder.CreateTable(
                name: "SETUPJOBHISTORY",
                columns: table => new
                {
                    No = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobID = table.Column<string>(maxLength: 50, nullable: false),
                    LotNo = table.Column<string>(maxLength: 50, nullable: false),
                    LD_START = table.Column<DateTime>(type: "DateTime", nullable: true),
                    LD_END = table.Column<DateTime>(type: "DateTime", nullable: true),
                    CT_START = table.Column<DateTime>(type: "DateTime", nullable: true),
                    CT_END = table.Column<DateTime>(type: "DateTime", nullable: true),
                    ST_START = table.Column<DateTime>(type: "DateTime", nullable: true),
                    ST_END = table.Column<DateTime>(type: "DateTime", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "DateTime", nullable: true),
                    CompletedOn = table.Column<DateTime>(type: "DateTime", nullable: true),
                    NoOfCarrier = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SETUPJOBHISTORY", x => x.No);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JOBDETAILSHISTORY");

            migrationBuilder.DropTable(
                name: "SETUPJOBHISTORY");
        }
    }
}
