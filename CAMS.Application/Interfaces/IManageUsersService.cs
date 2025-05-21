using CAMS.Application.DTOs;
using CAMS.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CAMS.Application.Interfaces
{
    public interface IManageUsersService
    {
        Task<List<UserDTO>> GetUsers();
        Task<List<string>> GetRoles(UserDTO userDTO);
        string GetUserId(ClaimsPrincipal user);
        Task<User> GetUser(ClaimsPrincipal user);
        Task<User> GetUserById(int id);
        Task UpdateUserLastActivityDate(ClaimsPrincipal user);
        Task changeRoleFromTo(User user, string oldRole, string NewRole);
        Task updateAsync(User user);
        int GetTotalUsers();

    }
}
