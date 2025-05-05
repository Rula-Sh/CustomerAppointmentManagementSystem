using BusinessLogicLayer.DTOs;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Interfaces
{
    public interface IManageUsersService
    {
        Task<List<UserDTO>> GetUsers();
        Task<List<string>> GetRoles(UserDTO userDTO);
        string GetUserId(ClaimsPrincipal user);
        Task<User> GetUserById(ClaimsPrincipal user);
        Task UpdateUserLastActivityDate(ClaimsPrincipal user);
        Task changeRoleFromTo(User user, string oldRole, string NewRole);
        Task updateAsync(User user);


        int GetTotalUsers();

    }
}
