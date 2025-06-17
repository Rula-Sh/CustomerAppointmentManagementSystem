using AutoMapper;
using CAMS.Application.DTOs;
using CAMS.Application.DTOs.Users;
using CAMS.Application.Interfaces;
using CAMS.Application.Services;
using CAMS.Data.Models;
using CAMS.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;

namespace CAMS.Web.Controllers;

public class AccountController : Controller
{
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;
    private readonly IManageUsersService _manageUsersService;
    private readonly IMapper _mapper;
    public AccountController(SignInManager<User> signInManager, UserManager<User> userManager, IManageUsersService manageUsersService, IMapper mapper)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _manageUsersService = manageUsersService;
        _mapper = mapper;

    }
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        var username = new EmailAddressAttribute().IsValid(model.Email) ? new MailAddress(model.Email).User : model.Email;

        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user != null)
            {
                if (!user.IsActive)
                {
                    ModelState.AddModelError(string.Empty, "Your account has been disabled.");
                    return View();
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "User does not exist.");
                return View();
            }


            var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Email or password is incorrect.");
                return View(model);
            }
        }
        return View(model);
    }
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            User user = new User
            {
                FullName = model.Name,
                Email = model.Email,
                UserName = model.Name,
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Customer");
                await _userManager.FindByNameAsync(user.UserName);
                var LoginResult = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
                if (LoginResult.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                return RedirectToAction("Login", "Account");
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
        return View(model);
    }

    public IActionResult VerifyEmail()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> VerifyEmail(VerifyEmailViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByNameAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError("", "Something is wrong!");
                return View(model);
            }
            else
            {
                return RedirectToAction("ChangePassword", "Account", new { username = user.UserName });
            }
        }
        return View(model);
    }

    public IActionResult ChangePassword(string username)
    {
        if (string.IsNullOrEmpty(username))
        {
            return RedirectToAction("VerifyEmail", "Account");
        }
        return View(new ChangePasswordViewModel { Email = username });
    }

    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByNameAsync(model.Email);
            if (user != null)
            {
                var result = await _userManager.RemovePasswordAsync(user);
                if (result.Succeeded)
                {
                    result = await _userManager.AddPasswordAsync(user, model.NewPassword);
                    return RedirectToAction("Login", "Account");
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
            else
            {
                ModelState.AddModelError("", "Email not found!");
                return View(model);
            }
        }
        else
        {
            ModelState.AddModelError("", "Something went wrong. try again.");
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        await _manageUsersService.UpdateUserLastActivityDate(User);
        var user = await _manageUsersService.GetUser(User);

        if (user == null)
            return RedirectToAction("Login");

        var userView = _mapper.Map<ProfileViewModel>(_mapper.Map<UserDTO>(user));
        return View(userView);
    }

    [HttpPost]
    public async Task<IActionResult> Profile(ProfileViewModel model)
    {
        await _manageUsersService.UpdateUserLastActivityDate(User);
        var user = await _manageUsersService.GetUser(User);

        if (user == null)
            return RedirectToAction("Login");

        if (!ModelState.IsValid)
            return View(model);

        if (model.ProfilePictureUpload != null && model.ProfilePictureUpload.Length > 0)
        {
            using var ms = new MemoryStream();
            await model.ProfilePictureUpload.CopyToAsync(ms);
            model.ProfilePicture = ms.ToArray();
        }

        var userDTO = _mapper.Map<UserDTO>(user);
        if (model.ProfilePictureUpload == null || model.ProfilePicture.Length <= 0)
        {
            model.ProfilePicture = userDTO.ProfilePicture;
        }
        userDTO = _mapper.Map(model,userDTO);

        await _manageUsersService.UpdateUserDetails(userDTO);
        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

}
