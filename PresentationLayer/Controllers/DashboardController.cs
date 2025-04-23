using DataAccessLayer.Data;
using DataAccessLayer.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PresentationLayer.Controllers
{
    [Authorize(Roles ="Admin")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        const string usersPath = "~/Views/Admin/Dashboard/Index.cshtml";

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var model = new DashboardViewModel
            {
                Users = _context.Users.ToList(),
                Services = _context.Services.ToList(),
                Appointments = _context.Appointments.ToList(),
            };

            return View(usersPath, model);
        }
    }
}
