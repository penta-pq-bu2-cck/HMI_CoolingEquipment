using Microsoft.EntityFrameworkCore.Migrations;

namespace HMI_CoolingEquipment.Migrations
{
    public partial class v1200 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SETUPJOB",
                table: "SETUPJOB");

            migrationBuilder.DropPrimaryKey(
                name: "PK_JOBDETAILS",
                table: "JOBDETAILS");

            migrationBuilder.DropColumn(
                name: "LotID",
                table: "SETUPJOB");

            migrationBuilder.DropColumn(
                name: "LotID",
                table: "JOBDETAILS");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SETUPJOB",
                table: "SETUPJOB",
                column: "JobID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_JOBDETAILS",
                table: "JOBDETAILS",
                columns: new[] { "JobID", "CarrierID" });

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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SETTINGS");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SETUPJOB",
                table: "SETUPJOB");

            migrationBuilder.DropPrimaryKey(
                name: "PK_JOBDETAILS",
                table: "JOBDETAILS");

            migrationBuilder.DropColumn(
                name: "StagingTime",
                table: "SETUPJOB");

            migrationBuilder.AddColumn<string>(
                name: "LotID",
                table: "SETUPJOB",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LotID",
                table: "JOBDETAILS",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SETUPJOB",
                table: "SETUPJOB",
                columns: new[] { "JobID", "LotID" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_JOBDETAILS",
                table: "JOBDETAILS",
                columns: new[] { "JobID", "LotID", "CarrierID" });
        }
    }
}
