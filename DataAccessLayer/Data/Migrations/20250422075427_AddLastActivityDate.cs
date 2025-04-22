using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddLastActivityDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastActivityDate",
                schema: "security",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()" // sets the value only for existing rows at the time of the migration.
                );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastActivityDate",
                schema: "security",
                table: "Users");
        }
    }
}
