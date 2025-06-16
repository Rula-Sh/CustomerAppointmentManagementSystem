using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CAMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameEmployeeToProvider : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Users_EmployeeId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Services_Users_EmployeeId",
                table: "Services");

            migrationBuilder.RenameColumn(
                name: "EmployeeId",
                table: "Services",
                newName: "ProviderId");

            migrationBuilder.RenameIndex(
                name: "IX_Services_EmployeeId",
                table: "Services",
                newName: "IX_Services_ProviderId");

            migrationBuilder.RenameColumn(
                name: "EmployeeId",
                table: "Appointments",
                newName: "ProviderId");

            migrationBuilder.RenameIndex(
                name: "IX_Appointments_EmployeeId",
                table: "Appointments",
                newName: "IX_Appointments_ProviderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Users_ProviderId",
                table: "Appointments",
                column: "ProviderId",
                principalSchema: "security",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Services_Users_ProviderId",
                table: "Services",
                column: "ProviderId",
                principalSchema: "security",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Users_ProviderId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Services_Users_ProviderId",
                table: "Services");

            migrationBuilder.RenameColumn(
                name: "ProviderId",
                table: "Services",
                newName: "EmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_Services_ProviderId",
                table: "Services",
                newName: "IX_Services_EmployeeId");

            migrationBuilder.RenameColumn(
                name: "ProviderId",
                table: "Appointments",
                newName: "EmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_Appointments_ProviderId",
                table: "Appointments",
                newName: "IX_Appointments_EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Users_EmployeeId",
                table: "Appointments",
                column: "EmployeeId",
                principalSchema: "security",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Services_Users_EmployeeId",
                table: "Services",
                column: "EmployeeId",
                principalSchema: "security",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
