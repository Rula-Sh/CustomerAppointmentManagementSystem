using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Data.Migrations
{
    /// <inheritdoc />
    public partial class AssignAdminUserToAllRolesFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // i assigned the admin to all roles because the relationship between Users and Roles are N:N
            migrationBuilder.Sql("INSERT INTO [security].[UserRoles] (UserId, RoleId) SELECT 1, Id FROM [security].[Roles];"); // assign Admin to all roles
            //migrationBuilder.Sql("INSERT INTO [security].[UserRoles] (UserId, RoleId) VALUES (1,1);"); /// assign admin to role admin
            // i cant use VALUES INTO because i need to get the values from another table... note that the SeedRoles migration create a new Guid every time i do a migration... so i need to get that id from the Roles table 
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM [security].[UserRoles] WHERE UserId= 1;");
        }
    }
}
