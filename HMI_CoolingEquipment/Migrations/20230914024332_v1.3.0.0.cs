using Microsoft.EntityFrameworkCore.Migrations;

namespace HMI_CoolingEquipment.Migrations
{
    public partial class v1300 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CarrierStatus",
                table: "JOBDETAILSHISTORY");

            migrationBuilder.DropColumn(
                name: "JobID",
                table: "JOBDETAILSHISTORY");

            migrationBuilder.DropColumn(
                name: "LotNo",
                table: "JOBDETAILSHISTORY");

            migrationBuilder.AddColumn<string>(
                name: "LoadBy",
                table: "JOBDETAILSHISTORY",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SETUPJOBHISTORYNo",
                table: "JOBDETAILSHISTORY",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UnloadBy",
                table: "JOBDETAILSHISTORY",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LoadBy",
                table: "JOBDETAILS",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UnloadBy",
                table: "JOBDETAILS",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_JOBDETAILSHISTORY_SETUPJOBHISTORYNo",
                table: "JOBDETAILSHISTORY",
                column: "SETUPJOBHISTORYNo");

            migrationBuilder.AddForeignKey(
                name: "FK_JOBDETAILSHISTORY_SETUPJOBHISTORY_SETUPJOBHISTORYNo",
                table: "JOBDETAILSHISTORY",
                column: "SETUPJOBHISTORYNo",
                principalTable: "SETUPJOBHISTORY",
                principalColumn: "No",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JOBDETAILSHISTORY_SETUPJOBHISTORY_SETUPJOBHISTORYNo",
                table: "JOBDETAILSHISTORY");

            migrationBuilder.DropIndex(
                name: "IX_JOBDETAILSHISTORY_SETUPJOBHISTORYNo",
                table: "JOBDETAILSHISTORY");

            migrationBuilder.DropColumn(
                name: "LoadBy",
                table: "JOBDETAILSHISTORY");

            migrationBuilder.DropColumn(
                name: "SETUPJOBHISTORYNo",
                table: "JOBDETAILSHISTORY");

            migrationBuilder.DropColumn(
                name: "UnloadBy",
                table: "JOBDETAILSHISTORY");

            migrationBuilder.DropColumn(
                name: "LoadBy",
                table: "JOBDETAILS");

            migrationBuilder.DropColumn(
                name: "UnloadBy",
                table: "JOBDETAILS");

            migrationBuilder.AddColumn<int>(
                name: "CarrierStatus",
                table: "JOBDETAILSHISTORY",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "JobID",
                table: "JOBDETAILSHISTORY",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LotNo",
                table: "JOBDETAILSHISTORY",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
