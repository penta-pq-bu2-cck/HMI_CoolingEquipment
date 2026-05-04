using Microsoft.EntityFrameworkCore.Migrations;

namespace HMI_CoolingEquipment.Migrations
{
    public partial class v4000 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SETUPJOB",
                table: "SETUPJOB");

            migrationBuilder.DropPrimaryKey(
                name: "PK_JOBDETAILS",
                table: "JOBDETAILS");

            migrationBuilder.AddColumn<string>(
                name: "LotNo",
                table: "SETUPJOB",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LotNo",
                table: "JOBDETAILS",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SETUPJOB",
                table: "SETUPJOB",
                columns: new[] { "JobID", "LotNo" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_JOBDETAILS",
                table: "JOBDETAILS",
                columns: new[] { "JobID", "LotNo", "CarrierID" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SETUPJOB",
                table: "SETUPJOB");

            migrationBuilder.DropPrimaryKey(
                name: "PK_JOBDETAILS",
                table: "JOBDETAILS");

            migrationBuilder.DropColumn(
                name: "LotNo",
                table: "SETUPJOB");

            migrationBuilder.DropColumn(
                name: "LotNo",
                table: "JOBDETAILS");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SETUPJOB",
                table: "SETUPJOB",
                column: "JobID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_JOBDETAILS",
                table: "JOBDETAILS",
                columns: new[] { "JobID", "CarrierID" });
        }
    }
}
