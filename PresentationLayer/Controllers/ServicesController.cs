using DataAccessLayer.Data;
using DataAccessLayer.Models;
using DataAccessLayer.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace PresentationLayer.Controllers
{
    [Authorize(Roles = "Admin, Employee")]

    public class ServicesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;


        public ServicesController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            /*var posts = await _context.Posts.OrderByDescending(m => m.DatePublished.Year).ToListAsync(); // this will get me the list of posts*/
            var services = await _context.Services.ToListAsync(); // this will get me the list of posts

            //OrderByDescending(m => m.DatePublished.Year).ToListAsync() will order the DatePublished of the posts from the top to the bottom
            return View(services);
        }

        public async Task<IActionResult> Add()
        {
            var viewModel = new ServiceViewModel{};
            return View(viewModel); // had to pass a viewModel so that i dont get an error in Add.cshtml where it expects PostFormViewModel object (System.NullReferenceException: 'Object reference not set to an instance of an object.'... Microsoft.AspNetCore.Mvc.Razor.RazorPage<TModel>.Model.get returned null.)
            //DatePicker.MinDate = DateTime.Now.ToString();
            //DatePicker.MaxDate = DateTime.Now.AddDays(30).ToString();
            //TimePicker.MinTime = "9:00:00";
            //TimePicker.MaxTime = "18:00:00";
            //return View();
        }
        [HttpPost]
        public async Task<IActionResult> Add(ServiceViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var service = new Service
            {
                Name = model.Name,
                Description = model.Description,
                // Add more properties if needed
            };
            _context.Services.Add(service);
            await _context.SaveChangesAsync();

            foreach (var dateStr in model.SelectedDates)
            {
                if (!DateTime.TryParse(dateStr, out var parsedDateTime))
                    continue;

                var serviceDate = new ServiceDate
                {
                    ServiceId = service.Id,
                    Date = DateOnly.FromDateTime(parsedDateTime)
                };
                _context.ServiceDates.Add(serviceDate);
                await _context.SaveChangesAsync();

                if (model.TimeSlots != null &&
                    model.TimeSlots.TryGetValue(dateStr, out var timeList))
                {
                    foreach (var timeStr in timeList)
                    {
                        if (TimeSpan.TryParse(timeStr, out var parsedTimeSpan))
                        {
                            var timeSlot = new ServiceTimeSlot
                            {
                                ServiceDateId = serviceDate.Id,
                                Time = TimeOnly.FromTimeSpan(parsedTimeSpan)
                            };
                            _context.ServiceTimeSlots.Add(timeSlot);
                        }
                    }
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }



    }
}
