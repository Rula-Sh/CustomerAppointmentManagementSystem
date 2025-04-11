using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAvailableTimeSlotsFromServicesAndChangeTimeInServiceTimeSlotTypeToString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvailableTimeSlots",
                table: "Services");

            migrationBuilder.AlterColumn<string>(
                name: "Time",
                table: "ServiceTimeSlots",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(TimeOnly),
                oldType: "time");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<TimeOnly>(
                name: "Time",
                table: "ServiceTimeSlots",
                type: "time",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "AvailableTimeSlots",
                table: "Services",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
