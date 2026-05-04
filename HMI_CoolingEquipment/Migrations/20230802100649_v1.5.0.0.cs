using Microsoft.EntityFrameworkCore.Migrations;

namespace HMI_CoolingEquipment.Migrations
{
    public partial class v1500 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.CreateTable(
            //    name: "BIN",
            //    columns: table => new
            //    {
            //        BinID = table.Column<string>(maxLength: 50, nullable: false),
            //        BinRack = table.Column<string>(maxLength: 50, nullable: false),
            //        BinColumn = table.Column<string>(maxLength: 50, nullable: false),
            //        BinRow = table.Column<string>(maxLength: 50, nullable: false),
            //        BinType = table.Column<string>(maxLength: 50, nullable: false),
            //        LoadDetails = table.Column<string>(maxLength: 50, nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_BIN", x => x.BinID);
            //    });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropTable(
            //    name: "BIN");
        }
    }
}
