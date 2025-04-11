using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("SET IDENTITY_INSERT [security].[Users] ON");
            migrationBuilder.Sql("INSERT INTO [security].[Users] ([Id], [FullName], [CreatedAt], [ProfilePicture], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount]) VALUES (1, N'admin admin', N'0001-01-01 00:00:00', NULL, N'admin', N'ADMIN', N'admin@admin.admin', N'ADMIN@ADMIN.ADMIN', 0, N'AQAAAAIAAYagAAAAEGpwCBNRxJ72Ci3VPPsAfKpcTHuwY/+70SBf16nb0Iex0UVAPLz/CkbKSDb8J1aUDw==', N'DEXOSCKZ4U6M46NA26AVK3GQL5672LAE', N'd9444a17-5a0e-4090-b8bf-acac2b2c65ec', NULL, 0, 0, NULL, 1, 0)");
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
