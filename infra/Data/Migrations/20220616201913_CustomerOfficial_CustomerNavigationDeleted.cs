using Microsoft.EntityFrameworkCore.Migrations;

namespace infra.Data.Migrations
{
    public partial class CustomerOfficial_CustomerNavigationDeleted : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerOfficials_Customers_CustomerId1",
                table: "CustomerOfficials");

            migrationBuilder.DropIndex(
                name: "IX_CustomerOfficials_CustomerId1",
                table: "CustomerOfficials");

            migrationBuilder.DropColumn(
                name: "CustomerId1",
                table: "CustomerOfficials");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CustomerId1",
                table: "CustomerOfficials",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerOfficials_CustomerId1",
                table: "CustomerOfficials",
                column: "CustomerId1");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerOfficials_Customers_CustomerId1",
                table: "CustomerOfficials",
                column: "CustomerId1",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
