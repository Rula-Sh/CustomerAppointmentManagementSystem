using BusinessLogicLayer.Interfaces;
using DataAccessLayer.Data;
using DataAccessLayer.Models;
using DataAccessLayer.Models.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class ManageUsers : IManageUsers
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;

        public ManageUsers(UserManager<User> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<List<User>> getUsers()
        {
            return await _userManager.Users.ToListAsync();
        }

        public async Task<List<string>> getRoles(User user)
        {
            return (List<string>)await _userManager.GetRolesAsync(user);
        }

        public string GetUserId(ClaimsPrincipal user)
        {
            return _userManager.GetUserId(user);
            // was using this in the controller: 'var userId = User.Identity.GetUserId();'
        }

        public async Task<User> GetUserById(ClaimsPrincipal user)
        {
            return await _userManager.FindByIdAsync(GetUserId(user));
            // OR:  _context.Users.SingleOrDefault(a => a.Id == userId);

        }

        public async Task UpdateUserLastActivityDate(ClaimsPrincipal user)
        {
            var currentUser = await GetUserById(user);
            if (currentUser != null)
            {
                currentUser.LastActivityDate = DateTime.Now; //.UtcNow
                _context.SaveChanges();
            }
        }
        public async Task changeRoleFromTo(User user, string oldRole,string NewRole)
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
