using Microsoft.EntityFrameworkCore.Migrations;

namespace HMI_CoolingEquipment.Migrations
{
    public partial class v3000 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LOADPORT");

            //migrationBuilder.CreateTable(
            //    name: "BIN",
            //    columns: table => new
            //    {
            //        BinID = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
            //        BinColumn = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
            //        BinError = table.Column<bool>(type: "bit", nullable: false),
            //        BinIPAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
            //        BinNodeAddress = table.Column<int>(type: "int", nullable: true),
            //        BinNodeConnectionStatus = table.Column<bool>(type: "bit", nullable: false),
            //        BinRack = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
            //        BinReadyToLoad = table.Column<bool>(type: "bit", nullable: false),
            //        BinReadyToUnload = table.Column<bool>(type: "bit", nullable: false),
            //        BinRow = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
            //        BinState = table.Column<int>(type: "int", nullable: false),
            //        BinType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
            //        LoadDetails = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
            //        PreAssignLoad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_BIN", x => x.BinID);
            //    });
        }
    }
}
