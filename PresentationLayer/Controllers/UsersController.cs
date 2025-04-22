using DataAccessLayer.Models;
using DataAccessLayer.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using BusinessLogicLayer.Helpers;
using BusinessLogicLayer.Interfaces;
using Microsoft.AspNet.Identity;

namespace PresentationLayer.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly IManageUsers _manageUsers;

        const string usersPath = "~/Views/Admin/Users/Index.cshtml";

        public UsersController(IManageUsers manageUsers)
        {
            _manageUsers = manageUsers;
        }

        public async Task<IActionResult> Index()
        {
            await _manageUsers.UpdateUserLastActivityDate(User);

            var users = await _manageUsers.getUsers();

            var userViewModels = new List<UserViewModel>();

            foreach (var user in users)
            {
                var roles = await _manageUsers.getRoles(user); // Await roles asynchronously

                TimeDifferenceHelper tdh = new TimeDifferenceHelper();
                var test = tdh.getTimeDifference(user.LastActivityDate);

                userViewModels.Add(new UserViewModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = user.FullName,
                    CreatedAt = user.CreatedAt,
                    IsActive = user.IsActive,
                    LastActivityDate = user.LastActivityDate,
                    LastActivity = tdh.getTimeDifference(user.LastActivityDate),
                    Roles = roles
                });
            }

            return View(usersPath, userViewModels);

        }

        [HttpPost]
        public async Task<IActionResult> ChangeAccountEmployement(int id)
        {
            var user = await _manageUsers.GetUserById(User);

            if (user == null)
                return NotFound();

            await _manageUsers.changeRoleFromTo(user, "Customer", "Employee");

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> changeAccountActivity(int id)
        {
            var user = await _manageUsers.GetUserById(User);

            if (user == null)
                return NotFound();

            user.IsActive = !user.IsActive;

            await _manageUsers.updateAsync(user);

            return RedirectToAction(nameof(Index));
        }
    }

}
