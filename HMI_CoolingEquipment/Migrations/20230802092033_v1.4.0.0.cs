using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HMI_CoolingEquipment.Migrations
{
    public partial class v1400 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "USERACCOUNT",
                type: "DateTime",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastLoginTime",
                table: "USERACCOUNT",
                type: "DateTime",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserGroup",
                table: "USERACCOUNT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "USERGROUPACCESS",
                columns: table => new
                {
                    UserGroup = table.Column<string>(maxLength: 50, nullable: false),
                    LotStatusPageAccess = table.Column<bool>(nullable: false),
                    LoadingPageAccess = table.Column<bool>(nullable: false),
                    UnloadingPageAccess = table.Column<bool>(nullable: false),
                    SecsGemPageAccess = table.Column<bool>(nullable: false),
                    SettingPageAccess = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USERGROUPACCESS", x => x.UserGroup);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "USERGROUPACCESS");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "USERACCOUNT");

            migrationBuilder.DropColumn(
                name: "LastLoginTime",
                table: "USERACCOUNT");

            migrationBuilder.DropColumn(
                name: "UserGroup",
                table: "USERACCOUNT");
        }
    }
}
