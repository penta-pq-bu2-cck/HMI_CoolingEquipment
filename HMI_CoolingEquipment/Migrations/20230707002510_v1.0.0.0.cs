using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HMI_CoolingEquipment.Migrations
{
    public partial class v1000 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SETUPJOB",
                columns: table => new
                {
                    JobID = table.Column<string>(maxLength: 50, nullable: false),
                    LotID = table.Column<string>(maxLength: 50, nullable: false),
                    CoolingTime = table.Column<string>(maxLength: 50, nullable: false),
                    StagingTime = table.Column<string>(maxLength: 50, nullable: false),
                    LoadingTime = table.Column<string>(maxLength: 50, nullable: false),
                    LD_START = table.Column<DateTime>(nullable: true),
                    LD_END = table.Column<DateTime>(nullable: true),
                    CT_START = table.Column<DateTime>(nullable: true),
                    CT_END = table.Column<DateTime>(nullable: true),
                    ST_START = table.Column<DateTime>(nullable: true),
                    ST_END = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SETUPJOB", x => new { x.JobID, x.LotID });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SETUPJOB");
        }
    }
}
