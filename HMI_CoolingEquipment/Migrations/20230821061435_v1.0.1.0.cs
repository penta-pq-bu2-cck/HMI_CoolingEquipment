using Microsoft.EntityFrameworkCore.Migrations;

namespace HMI_CoolingEquipment.Migrations
{
    public partial class v1010 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "JobStatus",
                table: "SETUPJOB",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NoOfCarrier",
                table: "SETUPJOB",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "CarrierStatus",
                table: "JOBDETAILS",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NoOfCarrier",
                table: "SETUPJOB");

            migrationBuilder.AlterColumn<string>(
                name: "JobStatus",
                table: "SETUPJOB",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<string>(
                name: "CarrierStatus",
                table: "JOBDETAILS",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(int));
        }
    }
}
