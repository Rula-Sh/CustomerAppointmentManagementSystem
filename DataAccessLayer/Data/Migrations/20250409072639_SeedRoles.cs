using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedRoles : Migration
    {
        /// <inheritdoc />
        // add Roles
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // create roles
            // create Admin role
            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Name", "NormalizedName", "ConcurrencyStamp" },
                values: new object[] { "Admin", "Admin".ToUpper(), Guid.NewGuid().ToString() },
                schema: "security" // i must add the schema or i will get an error searching for the default schema
                );
            // every time i run the migration it will generate a new Guid

            // create Employee role
            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Name", "NormalizedName", "ConcurrencyStamp" },
                values: new object[] {"Employee", "Employee".ToUpper(), Guid.NewGuid().ToString() },
                schema: "security"
                );

            // create Customer role
            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Name", "NormalizedName", "ConcurrencyStamp" },
                values: new object[] {"Customer", "Customer".ToUpper(), Guid.NewGuid().ToString() },
                schema: "security"
                );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.Sql("DELETE FROM [security].[Roles]");
        }

    }
}
