using Microsoft.EntityFrameworkCore.Migrations;

namespace HMI_CoolingEquipment.Migrations
{
    public partial class AddLoadPortConfig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LOADPORTCONFIGURATION",
                columns: table => new
                {
                    LoadPortStatusKey = table.Column<string>(maxLength: 50, nullable: false),
                    LoadPortStatusState = table.Column<int>(nullable: false),
                    LoadPortLEDColour = table.Column<int>(nullable: false),
                    LoadPortLEDState = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LOADPORTCONFIGURATION", x => x.LoadPortStatusKey);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LOADPORTCONFIGURATION");
        }
    }
}
