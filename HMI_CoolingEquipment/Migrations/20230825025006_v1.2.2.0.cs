using Microsoft.EntityFrameworkCore.Migrations;

namespace HMI_CoolingEquipment.Migrations
{
    public partial class v1220 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SETTINGS",
                table: "SETTINGS");

            migrationBuilder.AddColumn<bool>(
                name: "IsJobError",
                table: "SETUPJOB",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "SETTINGS",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<int>(
                name: "Page",
                table: "SETTINGS",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SETTINGS",
                table: "SETTINGS",
                columns: new[] { "Items", "Page" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SETTINGS",
                table: "SETTINGS");

            migrationBuilder.DropColumn(
                name: "IsJobError",
                table: "SETUPJOB");

            migrationBuilder.DropColumn(
                name: "Page",
                table: "SETTINGS");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "SETTINGS",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AddPrimaryKey(
                name: "PK_SETTINGS",
                table: "SETTINGS",
                column: "Items");
        }
    }
}
