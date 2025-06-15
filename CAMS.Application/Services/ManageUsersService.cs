using System.Collections.Generic;
using System.Security.Claims;
using AutoMapper;
using CAMS.Application.DTOs;
using CAMS.Application.Interfaces;
using CAMS.Data;
using CAMS.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CAMS.Application.Services
{
    public class ManageUsersService : IManageUsersService
    {
        private readonly UserManager<User> _userManager; // UserManager<User> is a class that provides APIs for managing users in an application. It is part of the ASP.NET Identity system, which is used to handle authentication, authorization, and user management.
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IAuditLogService _auditLogService;

        public ManageUsersService(UserManager<User> userManager, ApplicationDbContext context, IMapper mapper, IAuditLogService auditLogService)
        {
            _userManager = userManager;
            _context = context;
            _mapper = mapper;
            _auditLogService = auditLogService;

        }

        public async Task<List<UserDTO>> GetUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            var userDTOs = new List<UserDTO>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var dto = _mapper.Map<UserDTO>(user);
                dto.Roles = roles;
                userDTOs.Add(dto);
            }

            return userDTOs;
        }

        public async Task<List<string>> GetRoles(UserDTO userDTO)
        {
            var user = _mapper.Map<User>(userDTO);
            return (List<string>)await _userManager.GetRolesAsync(user);
        }

        public string GetUserId(ClaimsPrincipal user) // ClaimsPrincipal is a class used to hold the claims-based identity of a user and provides access to that user's information, such as their roles, permissions, and other identity-related claims.
        {
            return _userManager.GetUserId(user);
            // was using this in the controller: 'var userId = User.Identity.GetUserId();'
        }

        public async Task<User> GetUser(ClaimsPrincipal user)
        {
            return await _userManager.FindByIdAsync(GetUserId(user));
        }

        public async Task<User> GetUserById(int id)
        {
            return await _userManager.FindByIdAsync(id.ToString());
            // OR:  _context.Users.SingleOrDefault(a => a.Id == userId);
        }

        public async Task UpdateUserLastActivityDate(ClaimsPrincipal user)
        {
            var currentUser = await GetUser(user);
            if (currentUser != null)
            {
                currentUser.LastActivityDate = DateTime.Now; // or DateTime.UtcNow

                var entry = _context.Entry(currentUser);

                // Ensure the entity is tracked  (mark as modified only if it's detached or not already tracked)
                if (entry.State == EntityState.Detached)
                {
                    _context.Attach(currentUser);
                }

                entry.State = EntityState.Modified; // Mark as modified if it's detached or newly attached

                await _context.SaveChangesAsync();  // Save changes
            }
        }

        public async Task UpdateUserDetails(UserDTO userDTO)
        {
            var user = await _userManager.FindByIdAsync(userDTO.Id.ToString());

            if (user == null)
                throw new InvalidOperationException("User not found");

            user.UserName = userDTO.FullName;

            // var user = _mapper.Map<User>(userDTO); is WRONG Because i want to update user details not create a new one (it overrides all the properties)
            _mapper.Map(userDTO, user);

            await _context.SaveChangesAsync();
            //await _auditLogService.AddAuditLog(user.Id, $"{user.UserRoles.First()}", $"have updated his profile", "Update Profile");
        }

        public async Task changeRoleFromTo(User user, string oldRole, string NewRole)
        {
            if (oldRole == "Provider")
            {
                var services = await _context.Services.Include(s => s.ServiceDates).ThenInclude(sd => sd.ServiceTimeSlots).Where(s => s.ProviderId == user.Id).ToListAsync();
                if (services.Any())
                {
                    //services != null → ensures the variable is not null(i.e., the list exists).
                    //services.Any() → ensures the list contains at least one item to remove.
                    _context.Services.RemoveRange(services);
                    await _context.SaveChangesAsync();
                }

                await _auditLogService.AddAuditLog(1, "Admin", $"have fired {user.FullName} with ID: {user.Id}", "Fire Provider");
            }
            else if (oldRole == "Customer")
            {
                var appointments = await _context.Appointments.Where(s => s.CustomerId == user.Id).ToListAsync();
                if (appointments.Any())
                {
                    _context.Appointments.RemoveRange(appointments);
                    await _context.SaveChangesAsync();
                }

                await _auditLogService.AddAuditLog(1, "Admin", $"have hired {user.FullName} with ID: {user.Id}", "Hire Provider");
            }

            await _userManager.RemoveFromRoleAsync(user, oldRole);
            await _userManager.AddToRoleAsync(user, NewRole);
        }

        public async Task updateAsync(User user)
        {
            await _userManager.UpdateAsync(user);
        }

        public int GetTotalUsers()
        {
            return _context.Users.Count();
        }
    }
}
