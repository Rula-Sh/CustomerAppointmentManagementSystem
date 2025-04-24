using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Interfaces
{
    public interface IManageUsers
    {
        Task<List<User>> GetUsers();
        Task<List<string>> GetRoles(User user);
        string GetUserId(ClaimsPrincipal user);
        Task<User> GetUserById(ClaimsPrincipal user);
        Task UpdateUserLastActivityDate(ClaimsPrincipal user);
        Task changeRoleFromTo(User user, string oldRole, string NewRole);
        Task updateAsync(User user);


        int GetTotalUsers();

    }
}
