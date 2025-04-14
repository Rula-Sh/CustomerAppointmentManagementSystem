using DataAccessLayer.Data;
using DataAccessLayer.Models;
using DataAccessLayer.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel;

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
            //var services = await _context.Services.ToListAsync(); // this will get me the list of services

            //OrderByDescending(m => m.DatePublished.Year).ToListAsync() will order the DatePublished of the posts from the top to the bottom
            //return View(services);

            var services = await _context.Services.Select(s => new ServiceViewModel
            {
                Id = s.Id,
                Name = s.Name,
                Duration = s.Duration,
                Price = s.Price,
                Description = s.Description,
                DateTimeSlotGroups = s.ServiceDates.Select(d => new DateTimeSlotGroupViewModel
                {
                    Date = d.Date.ToString(),
                    TimeSlots = d.ServiceTimeSlots.Select(t => t.Time).ToList(),
                }).ToList()
            }).ToListAsync();

            return View(services);
        }

        public async Task<IActionResult> Add()
        {
            var viewModel = new ServiceViewModel { };
            return View(viewModel); // had to pass a viewModel so that i dont get an error in Add.cshtml where it expects PostFormViewModel object (System.NullReferenceException: 'Object reference not set to an instance of an object.'... Microsoft.AspNetCore.Mvc.Razor.RazorPage<TModel>.Model.get returned null.)

        }
        [HttpPost]
        public async Task<IActionResult> Add(ServiceViewModel model)
        {
            // make sure at least one time-slot is selected for each selected Date
            // no longer needed i added validation on form submit in Add.cshtml
            if (model.DateTimeSlotGroups == null || !model.DateTimeSlotGroups.Any(g => g.TimeSlots != null && g.TimeSlots.Any()))
            {
                // g here is each date group => { '11-05-2025': ['8:00'-'9:30','9:30'-'11:00',...], ...}
                // g.TimeSlots != null ensures the list is not null.
                // g.TimeSlots.Any() checks that the list contains at least one time-slot string.
                ModelState.AddModelError("", "At least one time slot for one date is required.");
                return View(model);
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // create Service
            var service = new Service
            {
                Name = model.Name,
                Description = model.Description,
                Duration = model.Duration,
                Price = model.Price,
                ServiceDates = new List<ServiceDate>()
            };

            // go through each DateTimeSlotGroup to get the all the dates and time-slots for each date
            foreach (var group in model.DateTimeSlotGroups)
            {
                // parse the date string into DateOnly for Date prop in ServiceDate
                if (!DateOnly.TryParseExact(group.Date, "dd-MM-yyyy", out var date))
                {
                    // out var date is compiled if the parse was successful, create the var date and set the parsed group.Date (Parsed to DateOnly) to date
                    ModelState.AddModelError("", $"Invalid date format: {group.Date}");
                    return View(model);
                }

                // create ServiceDate
                var serviceDate = new ServiceDate
                {
                    ServiceId = service.Id, // to link it to Services Table
                    Date = date,
                    ServiceTimeSlots = new List<ServiceTimeSlot>()
                };

                foreach (var time in group.TimeSlots)
                {
                    // create ServiceTimeSlot
                    serviceDate.ServiceTimeSlots.Add(new ServiceTimeSlot
                    {
                        ServiceDateId = serviceDate.Id, // to link it to ServiceDate Table
                        Time = time
                    });
                }

                service.ServiceDates.Add(serviceDate); // add ServiceDate list to Services Table
            }

            // update DB
            _context.Services.Add(service);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return BadRequest();

            var service = await _context.Services.Include(p => p.ServiceDates).ThenInclude(d => d.ServiceTimeSlots).SingleOrDefaultAsync(m => m.Id == id);

            if (service == null)
                return NotFound();

            return View("Details", service);
        }
    }
}
