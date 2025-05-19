using AutoMapper;
using CAMS.Application.Helpers;
using CAMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CAMS.Web.ViewModels;
using CAMS.Data;

namespace CAMS.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly IManageUsersService _manageUsers;
        private readonly IManageServicesService _manageServices;
        private readonly IMapper _mapper;
        private readonly IAuditLogService _auditLogService;

        const string usersPath = "~/Views/Admin/Users/Index.cshtml";

        public UsersController(IManageUsersService manageUsers, IMapper mapper, IManageServicesService manageServices,IAuditLogService auditLogService)
        {
            _manageUsers = manageUsers;
            _mapper = mapper;
            _manageServices = manageServices;
            _auditLogService = auditLogService;

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
        [Route("Home/LoadUsers")]
        public async Task<IActionResult> LoadUsers()
        {
            using (ApplicationDbContext appDBC = new ApplicationDbContext())
            {
                var users = await _manageUsers.GetUsers();
                var tableData = users.Select(a => new {
                    a.Id,
                    a.FullName,
                    a.Email,
                    a.Roles,
                    a.LastActivity,
                    a.IsActive,
                });

                return Json(new { data = tableData });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ChangeAccountEmployement(int id, string from,string to)
        {
            var user = await _manageUsers.GetUserById(id);

            if (user == null)
                return NotFound();


            //await _manageUsers.changeRoleFromTo(user, from, to);
            //return RedirectToAction(nameof(Index));

            if (await _manageServices.doesTheUserHaveActiveAppointments(id))
            {
                return Ok(new { success = false, message = "The User Have Active Appointments, Please Wait for Them to be Completed!" });
            }
            else
            {
                await _manageUsers.changeRoleFromTo(user, from, to);
                return Ok(new { success = true, message = "The User Have Active Appointments, Please Wait for Them to be Completed!" });
            }

        }

        [HttpPost]
        public async Task<IActionResult> changeAccountActivity(int id)
        {
            var user = await _manageUsers.GetUserById(id);

            if (user == null)
                return NotFound();

            user.IsActive = !user.IsActive;

            if (user.IsActive)
            {
                await _auditLogService.AddAuditLog(1, "Admin", $"have activated {user.FullName} with ID: {user.Id} account", "Activate Account");
            }
            else
            {
                await _auditLogService.AddAuditLog(1, "Admin", $"have deactivated {user.FullName} with ID: {user.Id} account", "Deactivate Account");
            }

            await _manageUsers.updateAsync(user);

            return RedirectToAction(nameof(Index));
        }
    }

}
