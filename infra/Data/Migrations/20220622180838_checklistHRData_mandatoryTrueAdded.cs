using Microsoft.EntityFrameworkCore.Migrations;

namespace infra.Data.Migrations
{
    public partial class checklistHRData_mandatoryTrueAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LoggedInUserName",
                table: "ChecklistHRs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "MandatoryTrue",
                table: "ChecklistHRDatas",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LoggedInUserName",
                table: "ChecklistHRs");

            migrationBuilder.DropColumn(
                name: "MandatoryTrue",
                table: "ChecklistHRDatas");
        }
    }
}
