using AutoMapper;
using CAMS.Application.Helpers;
using CAMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CAMS.Web.ViewModels;
using CAMS.Data;
using CAMS.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace CAMS.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly IManageUsersService _manageUsers;
        private readonly IManageServicesService _manageServices;
        private readonly IMapper _mapper;
        private readonly IAuditLogService _auditLogService;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        const string usersPath = "~/Views/Admin/Users/Index.cshtml";

        public UsersController(IManageUsersService manageUsers, IMapper mapper, IManageServicesService manageServices, IAuditLogService auditLogService, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _manageUsers = manageUsers;
            _mapper = mapper;
            _manageServices = manageServices;
            _auditLogService = auditLogService;
            _userManager = userManager;
            _signInManager = signInManager;
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
                userViewModel.Roles = await _manageUsers.GetRoles(user);
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
                var tableData = users.Select(a => new
                {
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
        public async Task<IActionResult> ChangeAccountEmployement(int id, string from, string to)
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
                return Ok(new { success = true, message = "Sucess!" });
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

        [HttpGet]
        public IActionResult AddProvider()
        {
            var vm = new AddProviderViewModel();
            return View(vm); // remember that i had to return a AddProviderViewModel so that i dont get a null error in the if statement "@if (Model.ProfilePicture != null && Model.ProfilePicture.Length > 0)"... the Model is  null because the action that renders the page never supplies a view‑model instance.
        }

        [HttpPost]
        public async Task<IActionResult> AddProvider(AddProviderViewModel model)
        {

            if (model.ProfilePictureUpload != null && model.ProfilePictureUpload.Length > 0)
            {
                using var ms = new MemoryStream();
                await model.ProfilePictureUpload.CopyToAsync(ms);
                model.ProfilePicture = ms.ToArray();
                ModelState.Remove("ProfilePicture");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            User user = new User
            {
                FullName = model.Name,
                Email = model.Email,
                UserName = model.Name,
                PhoneNumber = model.PhoneNumber,
                ProfilePicture = model.ProfilePicture,
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Provider");
                await _userManager.FindByNameAsync(user.UserName);

                return RedirectToAction("Index", "Users");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }
        }
    }

}
