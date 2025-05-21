using CAMS.Application.Interfaces;
using CAMS.Data;
using CAMS.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAMS.Application.Services
{
    public class AuditLogService : IAuditLogService
    {

        private readonly ApplicationDbContext _context;
        private readonly Lazy<IManageUsersService> _manageUsersService;

        public AuditLogService(ApplicationDbContext context, Lazy<IManageUsersService> manageUsersService)
        {
            _context = context;
            _manageUsersService = manageUsersService;
        }

        public async Task AddAuditLog(int performerId, string role, string actionDescription, string actionType)
        {
            var user = await _manageUsersService.Value.GetUserById(performerId);

            var auditLog = new AuditLog
            {
                PerformerId = performerId,
                actionDescription = $"{role} \'{user.FullName}\' with ID: {performerId}, {actionDescription} at {DateTime.UtcNow}",
                ActionType = actionType,
                Time = DateTime.Now
            };
            _context.AuditLogs.Add(auditLog);
            _context.SaveChanges();
        }
    }
}
