using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BusinessLogicLayer.Helpers;
using BusinessLogicLayer.Interfaces;
using AutoMapper;
using PresentationLayer.ViewModels;

namespace PresentationLayer.Controllers
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
                /*
                // before making the getTimeDifference method to static 
                TimeDifferenceHelper tdh = new TimeDifferenceHelper();
                var test = tdh.getTimeDifference(user.LastActivityDate);

                var roles = await _manageUsers.GetRoles(user); // Await roles asynchronously
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
                });*/
                // using AutoMapper
                var userViewModel = _mapper.Map<UserViewModel>(user);
                userViewModel.LastActivity = TimeDifferenceHelper.getTimeDifference(user.LastActivityDate);
                userViewModel.Roles = await _manageUsers.GetRoles(user);// Await roles asynchronously
                usersViewModels.Add(userViewModel);
            }

            return View(usersPath, usersViewModels);

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
