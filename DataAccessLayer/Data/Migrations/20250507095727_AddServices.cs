using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddServices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // --------------------------- Services ---------------------------
            migrationBuilder.Sql("SET IDENTITY_INSERT [dbo].[Services] ON");
            migrationBuilder.Sql("INSERT INTO [dbo].[Services] ([Id], [Name], [Description], [Price], [Duration]) VALUES (1, N'1', N'1', CAST(1.00 AS Decimal(5, 2)), N'00:30')");
            migrationBuilder.Sql("INSERT INTO [dbo].[Services] ([Id], [Name], [Description], [Price], [Duration]) VALUES (2, N'2', N'2', CAST(2.00 AS Decimal(5, 2)), N'01:00')");
            migrationBuilder.Sql("INSERT INTO [dbo].[Services] ([Id], [Name], [Description], [Price], [Duration]) VALUES (3, N'3', N'3', CAST(3.00 AS Decimal(5, 2)), N'00:30')");
            migrationBuilder.Sql("SET IDENTITY_INSERT [dbo].[Services] OFF");


            // --------------------------- ServiceDates ---------------------------
            migrationBuilder.Sql("SET IDENTITY_INSERT [dbo].[ServiceDates] ON");
            migrationBuilder.Sql("INSERT INTO [dbo].[ServiceDates] ([Id], [ServiceId], [Date]) VALUES (1, 1, N'2025-01-01')");
            migrationBuilder.Sql("INSERT INTO [dbo].[ServiceDates] ([Id], [ServiceId], [Date]) VALUES (2, 1, N'2025-01-22')");
            migrationBuilder.Sql("INSERT INTO [dbo].[ServiceDates] ([Id], [ServiceId], [Date]) VALUES (3, 2, N'2025-04-09')");
            migrationBuilder.Sql("INSERT INTO [dbo].[ServiceDates] ([Id], [ServiceId], [Date]) VALUES (4, 2, N'2025-04-16')");
            migrationBuilder.Sql("INSERT INTO [dbo].[ServiceDates] ([Id], [ServiceId], [Date]) VALUES (5, 3, N'2025-04-14')");
            migrationBuilder.Sql("INSERT INTO [dbo].[ServiceDates] ([Id], [ServiceId], [Date]) VALUES (6, 3, N'2025-04-10')");
            migrationBuilder.Sql("INSERT INTO [dbo].[ServiceDates] ([Id], [ServiceId], [Date]) VALUES (7, 3, N'2025-04-08')");
            migrationBuilder.Sql("SET IDENTITY_INSERT [dbo].[ServiceDates] OFF");


            // --------------------------- ServiceTimeSlots ---------------------------
            migrationBuilder.Sql("SET IDENTITY_INSERT [dbo].[ServiceTimeSlots] ON");
            migrationBuilder.Sql("INSERT INTO [dbo].[ServiceTimeSlots] ([Id], [ServiceDateId], [Time]) VALUES(1, 1, N'14:00 - 14:30')");
            migrationBuilder.Sql("INSERT INTO [dbo].[ServiceTimeSlots] ([Id], [ServiceDateId], [Time]) VALUES(2, 1, N'13:30 - 14:00')");
            migrationBuilder.Sql("INSERT INTO [dbo].[ServiceTimeSlots] ([Id], [ServiceDateId], [Time]) VALUES(3, 2, N'11:30 - 12:00')");
                                              
            migrationBuilder.Sql("INSERT INTO [dbo].[ServiceTimeSlots] ([Id], [ServiceDateId], [Time]) VALUES(4, 3, N'10:00 - 11:00')");
            migrationBuilder.Sql("INSERT INTO [dbo].[ServiceTimeSlots] ([Id], [ServiceDateId], [Time]) VALUES(5, 3, N'09:00 - 10:00')");
                                              
            migrationBuilder.Sql("INSERT INTO [dbo].[ServiceTimeSlots] ([Id], [ServiceDateId], [Time]) VALUES(6, 4, N'12:00 - 13:00')");
            migrationBuilder.Sql("INSERT INTO [dbo].[ServiceTimeSlots] ([Id], [ServiceDateId], [Time]) VALUES(7, 4, N'12:00 - 13:00')");
                                              
            migrationBuilder.Sql("INSERT INTO [dbo].[ServiceTimeSlots] ([Id], [ServiceDateId], [Time]) VALUES(8, 5, N'08:30 - 09:00')");
            migrationBuilder.Sql("INSERT INTO [dbo].[ServiceTimeSlots] ([Id], [ServiceDateId], [Time]) VALUES(9, 5, N'10:00 - 10:30')");
            migrationBuilder.Sql("INSERT INTO [dbo].[ServiceTimeSlots] ([Id], [ServiceDateId], [Time]) VALUES(10, 5, N'10:30 - 11:00')");
            migrationBuilder.Sql("INSERT INTO [dbo].[ServiceTimeSlots] ([Id], [ServiceDateId], [Time]) VALUES(11, 5, N'13:00 - 13:30')");
            migrationBuilder.Sql("INSERT INTO [dbo].[ServiceTimeSlots] ([Id], [ServiceDateId], [Time]) VALUES(12, 5, N'13:30 - 15:00')");
                                              
            migrationBuilder.Sql("INSERT INTO [dbo].[ServiceTimeSlots] ([Id], [ServiceDateId], [Time]) VALUES(13, 6, N'08:00 - 08:30')");
            migrationBuilder.Sql("INSERT INTO [dbo].[ServiceTimeSlots] ([Id], [ServiceDateId], [Time]) VALUES(14, 6, N'08:30 - 09:00')");
            migrationBuilder.Sql("INSERT INTO [dbo].[ServiceTimeSlots] ([Id], [ServiceDateId], [Time]) VALUES(15, 6, N'09:30 - 10:00')");
                                              
            migrationBuilder.Sql("INSERT INTO [dbo].[ServiceTimeSlots] ([Id], [ServiceDateId], [Time]) VALUES(16, 7, N'08:30 - 09:00')");
            migrationBuilder.Sql("INSERT INTO [dbo].[ServiceTimeSlots] ([Id], [ServiceDateId], [Time]) VALUES(17, 7, N'09:00 - 09:30')");
            migrationBuilder.Sql("INSERT INTO [dbo].[ServiceTimeSlots] ([Id], [ServiceDateId], [Time]) VALUES(18, 7, N'09:30 - 10:00')");
            migrationBuilder.Sql("INSERT INTO [dbo].[ServiceTimeSlots] ([Id], [ServiceDateId], [Time]) VALUES(19, 7, N'10:30 - 11:00')");
            migrationBuilder.Sql("SET IDENTITY_INSERT [dbo].[ServiceTimeSlots] OFF");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM [dbo].[ServiceTimeSlots]");
            migrationBuilder.Sql("DELETE FROM [dbo].[ServiceDates]");
            migrationBuilder.Sql("DELETE FROM [dbo].[Services]");
        }
    }
}