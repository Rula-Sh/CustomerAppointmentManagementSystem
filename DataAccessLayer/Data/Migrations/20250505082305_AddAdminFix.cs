using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("SET IDENTITY_INSERT [security].[Users] ON");
            migrationBuilder.Sql("INSERT INTO [security].[Users] ([Id], [FullName], [CreatedAt], [ProfilePicture], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [IsActive], [LastActivityDate]) VALUES (1, N'admin admin', N'2025-05-05 11:23:55', NULL, N'admin', N'ADMIN', N'admin@admin.admin', N'ADMIN@ADMIN.ADMIN', 0, N'AQAAAAIAAYagAAAAEMG34b5tIWZsRF5wfrJ2uja9YaiMPqTurJ/yEetSraP9W1XRxHEiemHF4/fVn3yZuA==', N'U77IPDS4RGLYIH2IUQQHB6QNNWANIQP7', N'44304d7b-d58e-4b10-8969-3569f1d3b0ca', NULL, 0, 0, NULL, 1, 0, 1, N'2025-05-05 11:23:56')");
            migrationBuilder.Sql("SET IDENTITY_INSERT [security].[Users] OFF");

            //I got this script from the Users table, i viewed it, right clicked on the table - Script, and copied the new created user query 'admin in this case'... replaced "<SQLVARIANT>" with "Null" because the user does not have a profile picture

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM [security].[Users] WHERE Id = 1");
        }
    }
}
