using Microsoft.EntityFrameworkCore.Migrations;

namespace HMI_CoolingEquipment.Migrations
{
    public partial class RemoveLoadPortConfig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LOADPORTCONFIGURATION");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LOADPORTCONFIGURATION",
                columns: table => new
                {
                    LoadPortStatusKey = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LoadPortLEDColour = table.Column<int>(type: "int", nullable: false),
                    LoadPortLEDState = table.Column<int>(type: "int", nullable: false),
                    LoadPortStatusState = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LOADPORTCONFIGURATION", x => x.LoadPortStatusKey);
                });
        }
    }
}
