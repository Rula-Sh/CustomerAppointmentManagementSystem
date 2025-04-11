using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Data.Migrations
{
    /// <inheritdoc />
    public partial class SetDurationToStringInServices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DurationIn",
                table: "Services");

            migrationBuilder.AddColumn<string>(
                name: "Duration",
                table: "Services",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Duration",
                table: "Services");

            migrationBuilder.AddColumn<int>(
                name: "DurationIn",
                table: "Services",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
