using AutoMapper;
using CAMS.Application.Helpers;
using CAMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CAMS.Web.ViewModels;

namespace CAMS.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly IManageUsersService _manageUsers;
        private readonly IMapper _mapper;

        const string usersPath = "~/Views/Admin/Users/Index.cshtml";

        public UsersController(IManageUsersService manageUsers, IMapper mapper)
        {
            _manageUsers = manageUsers;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            await _manageUsers.UpdateUserLastActivityDate(User);

            var users = await _manageUsers.GetUsers();

            var usersViewModels = new List<UserViewModel>();

            foreach (var user in users)
            {              
                // using AutoMapper
                var userViewModel = _mapper.Map<UserViewModel>(user);
                userViewModel.LastActivity = TimeDifferenceHelper.getTimeDifference(user.LastActivityDate);
                userViewModel.Roles = await _manageUsers.GetRoles(user);// Await roles asynchronously
                usersViewModels.Add(userViewModel);
            }

            return View(usersPath, usersViewModels);

        }

        [HttpPost]
        public async Task<IActionResult> ChangeAccountEmployement(int id, string from,string to)
        {
            var user = await _manageUsers.GetUserById(id);

            if (user == null)
                return NotFound();

            await _manageUsers.changeRoleFromTo(user, from, to);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> changeAccountActivity(int id)
        {
            var user = await _manageUsers.GetUserById(id);

            if (user == null)
                return NotFound();

            user.IsActive = !user.IsActive;

            await _manageUsers.updateAsync(user);

            return RedirectToAction(nameof(Index));
        }
    }

}
