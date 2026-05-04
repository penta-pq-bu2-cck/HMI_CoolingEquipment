using Microsoft.EntityFrameworkCore.Migrations;

namespace HMI_CoolingEquipment.Migrations
{
    public partial class v1231 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LoadingTime",
                table: "SETUPJOB");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LoadingTime",
                table: "SETUPJOB",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
