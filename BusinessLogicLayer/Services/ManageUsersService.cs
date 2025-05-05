using AutoMapper;
using BusinessLogicLayer.DTOs;
using BusinessLogicLayer.Interfaces;
using DataAccessLayer.Data;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BusinessLogicLayer.Services
{
    public class ManageUsersService : IManageUsersService
    {
        private readonly UserManager<User> _userManager; // UserManager<User> is a class that provides APIs for managing users in an application. It is part of the ASP.NET Identity system, which is used to handle authentication, authorization, and user management.
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ManageUsersService(UserManager<User> userManager, ApplicationDbContext context, IMapper mapper)
        {
            _userManager = userManager;
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<UserDTO>> GetUsers()
        {
            var user = await _userManager.Users.ToListAsync();
            return _mapper.Map<List<UserDTO>>(user);
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

        public async Task<User> GetUserById(ClaimsPrincipal user)
        {
            return await _userManager.FindByIdAsync(GetUserId(user));
            // OR:  _context.Users.SingleOrDefault(a => a.Id == userId);
            //return _mapper.Map<UserDTO>(gottenUser);

        }

        public async Task UpdateUserLastActivityDate(ClaimsPrincipal user)
        {
            var currentUser = await GetUserById(user);
            if (currentUser != null)
            {
                currentUser.LastActivityDate = DateTime.Now; // or DateTime.UtcNow if needed

                // Ensure the entity is tracked
                var entry = _context.Entry(currentUser);
                if (entry.State == EntityState.Detached)
                {
                    _context.Attach(currentUser);  // Attach if it's detached
                    // Mark the entity as modified if necessary
                    _context.Entry(currentUser).State = EntityState.Modified;

                    await _context.SaveChangesAsync();
                }
            }
        }
        public async Task changeRoleFromTo(User user, string oldRole, string NewRole)
        {
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
