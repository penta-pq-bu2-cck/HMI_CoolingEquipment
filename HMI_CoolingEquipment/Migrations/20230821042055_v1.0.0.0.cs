using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HMI_CoolingEquipment.Migrations
{
    public partial class v1000 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JOBDETAILS",
                columns: table => new
                {
                    JobID = table.Column<string>(maxLength: 50, nullable: false),
                    LotNo = table.Column<string>(maxLength: 50, nullable: false),
                    CarrierID = table.Column<string>(maxLength: 50, nullable: false),
                    PortLocation = table.Column<string>(maxLength: 50, nullable: true),
                    LoadingTime = table.Column<DateTime>(type: "DateTime", nullable: true),
                    UnloadingTime = table.Column<DateTime>(type: "DateTime", nullable: true),
                    CarrierStatus = table.Column<string>(maxLength: 50, nullable: true),
                    Type = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JOBDETAILS", x => new { x.JobID, x.LotNo, x.CarrierID });
                });

            migrationBuilder.CreateTable(
                name: "LOADPORT",
                columns: table => new
                {
                    LoadPortID = table.Column<string>(maxLength: 50, nullable: false),
                    LoadPortRack = table.Column<string>(maxLength: 50, nullable: false),
                    LoadPortColumn = table.Column<string>(maxLength: 50, nullable: false),
                    LoadPortRow = table.Column<string>(maxLength: 50, nullable: false),
                    LoadPortType = table.Column<string>(maxLength: 50, nullable: false),
                    LoadPortCarrierLoad = table.Column<string>(maxLength: 50, nullable: true),
                    LoadPortIPAddress = table.Column<string>(maxLength: 50, nullable: true),
                    LoadPortNodeAddress = table.Column<int>(nullable: true),
                    LoadPortNodeConnectionStatus = table.Column<bool>(nullable: false),
                    LoadPortError = table.Column<bool>(nullable: false),
                    LoadPortState = table.Column<int>(nullable: false),
                    PreAssignLoad = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LOADPORT", x => x.LoadPortID);
                });

            migrationBuilder.CreateTable(
                name: "SETTINGS",
                columns: table => new
                {
                    Items = table.Column<string>(maxLength: 50, nullable: false),
                    Value = table.Column<string>(maxLength: 50, nullable: false),
                    Type = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SETTINGS", x => x.Items);
                });

            migrationBuilder.CreateTable(
                name: "SETUPJOB",
                columns: table => new
                {
                    JobID = table.Column<string>(maxLength: 50, nullable: false),
                    LotNo = table.Column<string>(maxLength: 50, nullable: false),
                    CoolingTime = table.Column<string>(maxLength: 50, nullable: false),
                    StagingTime = table.Column<string>(maxLength: 50, nullable: false),
                    LoadingTime = table.Column<string>(maxLength: 50, nullable: false),
                    LD_START = table.Column<DateTime>(type: "DateTime", nullable: true),
                    LD_END = table.Column<DateTime>(type: "DateTime", nullable: true),
                    CT_START = table.Column<DateTime>(type: "DateTime", nullable: true),
                    CT_END = table.Column<DateTime>(type: "DateTime", nullable: true),
                    ST_START = table.Column<DateTime>(type: "DateTime", nullable: true),
                    ST_END = table.Column<DateTime>(type: "DateTime", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "DateTime", nullable: true),
                    CompletedOn = table.Column<DateTime>(type: "DateTime", nullable: true),
                    JobStatus = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SETUPJOB", x => new { x.JobID, x.LotNo });
                });

            migrationBuilder.CreateTable(
                name: "USERACCOUNT",
                columns: table => new
                {
                    UserName = table.Column<string>(maxLength: 50, nullable: false),
                    Password = table.Column<string>(maxLength: 50, nullable: false),
                    FirstName = table.Column<string>(maxLength: 50, nullable: false),
                    LastName = table.Column<string>(maxLength: 50, nullable: false),
                    UserGroup = table.Column<string>(maxLength: 50, nullable: false),
                    LastLoginTime = table.Column<DateTime>(type: "DateTime", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "DateTime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USERACCOUNT", x => x.UserName);
                });

            migrationBuilder.CreateTable(
                name: "USERGROUPACCESS",
                columns: table => new
                {
                    UserGroup = table.Column<string>(maxLength: 50, nullable: false),
                    LotStatusPageAccess = table.Column<bool>(nullable: false),
                    LoadingPageAccess = table.Column<bool>(nullable: false),
                    UnloadingPageAccess = table.Column<bool>(nullable: false),
                    SecsGemPageAccess = table.Column<bool>(nullable: false),
                    SettingPageAccess = table.Column<bool>(nullable: false),
                    EditUserGroupAccess = table.Column<bool>(nullable: false),
                    EditUserAccountList = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USERGROUPACCESS", x => x.UserGroup);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JOBDETAILS");

            migrationBuilder.DropTable(
                name: "LOADPORT");

            migrationBuilder.DropTable(
                name: "SETTINGS");

            migrationBuilder.DropTable(
                name: "SETUPJOB");

            migrationBuilder.DropTable(
                name: "USERACCOUNT");

            migrationBuilder.DropTable(
                name: "USERGROUPACCESS");
        }
    }
}
