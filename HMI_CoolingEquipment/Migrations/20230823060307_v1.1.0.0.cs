using Microsoft.EntityFrameworkCore.Migrations;

namespace HMI_CoolingEquipment.Migrations
{
    public partial class v1100 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ALARMDEFINITION",
                columns: table => new
                {
                    AlarmType = table.Column<int>(maxLength: 50, nullable: false),
                    AlarmCode = table.Column<int>(nullable: false),
                    AlarmModule = table.Column<string>(nullable: true),
                    AlarmMessage = table.Column<string>(nullable: true),
                    AlarmAction = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ALARMDEFINITION", x => new { x.AlarmType, x.AlarmCode });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ALARMDEFINITION");
        }
    }
}
