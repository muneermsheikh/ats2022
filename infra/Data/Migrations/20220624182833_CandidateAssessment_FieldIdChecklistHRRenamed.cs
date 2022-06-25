using Microsoft.EntityFrameworkCore.Migrations;

namespace infra.Data.Migrations
{
    public partial class CandidateAssessment_FieldIdChecklistHRRenamed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChecklistHRs_Candidates_CandidateId",
                table: "ChecklistHRs");

            migrationBuilder.DropForeignKey(
                name: "FK_ChecklistHRs_OrderItems_OrderItemId",
                table: "ChecklistHRs");

            migrationBuilder.RenameColumn(
                name: "HrChecklistId",
                table: "CandidateAssessments",
                newName: "ChecklistHRId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ChecklistHRId",
                table: "CandidateAssessments",
                newName: "HrChecklistId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChecklistHRs_Candidates_CandidateId",
                table: "ChecklistHRs",
                column: "CandidateId",
                principalTable: "Candidates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChecklistHRs_OrderItems_OrderItemId",
                table: "ChecklistHRs",
                column: "OrderItemId",
                principalTable: "OrderItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
