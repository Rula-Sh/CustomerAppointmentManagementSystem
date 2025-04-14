using DataAccessLayer.Data;
using DataAccessLayer.Models;
using DataAccessLayer.Models.ViewModel;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;


namespace PresentationLayer.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly Microsoft.AspNetCore.Identity.UserManager<User> _userManager;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context,
        Microsoft.AspNetCore.Identity.UserManager<User> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            var userId = Int32.Parse(User.Identity.GetUserId());
            var appointmentsQuery = _context.Appointments.AsQueryable();
            if(User.IsInRole("Employee"))
            {
                appointmentsQuery = appointmentsQuery.Where(a => a.EmployeeId == userId);
            }else if (User.IsInRole("Customer"))
            {
                appointmentsQuery = appointmentsQuery.Where(a => a.CustomerId == userId);
            } 

            var appointments = await appointmentsQuery.Select(a => new AppoitmentViewModel
                {
                    Id = a.Id,
                    CustomerId = a.CustomerId,
                    CustomerName = a.Customer.FullName,
                    EmployeeId = a.EmployeeId,
                    Name = a.Name,
                    Date = a.Date,
                    Notes = a.Notes,
                    Status = a.Status,
                    CreatedAt = a.CreatedAt,
                }).ToListAsync();
            return View(appointments);
        }

        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Add()
        {
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

            var viewModel = new BookAppointmentViewModel
            {
                Services = services,
            };
            return View(viewModel); // had to pass a viewModel so that i dont get an error in Add.cshtml where it expects PostFormViewModel object (System.NullReferenceException: 'Object reference not set to an instance of an object.'... Microsoft.AspNetCore.Mvc.Razor.RazorPage<TModel>.Model.get returned null.)

        }

        [HttpGet]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetServiceDetails(int serviceId)
        {

            var service =await _context.Services.Include(p => p.ServiceDates).ThenInclude(d => d.ServiceTimeSlots).SingleOrDefaultAsync(m => m.Id == serviceId);

            if (service == null)
                return NotFound();

            var serviceViewModel = new ServiceViewModel
            {
                Id = service.Id,
                Name = service.Name,
                Price = service.Price,
                Description = service.Description,
                DateTimeSlotGroups = service.ServiceDates.Select(d => new DateTimeSlotGroupViewModel
                {
                    Date = d.Date.ToString(),
                    TimeSlots = d.ServiceTimeSlots.Select(t => t.Time).ToList(),
                }).ToList()
            };

            return Json(serviceViewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Add(BookAppointmentViewModel model)
        {
            ModelState.Remove("Services");
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);

            // create Service
            var appointment = new Appointment
            {

                CustomerId = Int32.Parse(User.Identity.GetUserId()),
                EmployeeId = 1, // had to set the employee Id to 1, so that i dont get a FOREIGN KEY constraint error.. couldnt either set it nullable
                ServiceId = model.ServiceId,
                Name = model.ServiceName,
                Date = model.Date,
                Status = "Pending",
                CreatedAt = DateTime.Now,
                //AppointmentServices = new List<AppointmentService>()
                Notes = "",
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }


        public async Task<IActionResult> PendingAppointments()
        {
            var userId = Int32.Parse(User.Identity.GetUserId());

            var appointments = await _context.Appointments.Select(a => new AppoitmentViewModel
            {
                Id = a.Id,
                CustomerId = a.CustomerId,
                CustomerName = a.Customer.FullName,
                EmployeeId = a.EmployeeId,
                Name = a.Name,
                Date = a.Date,
                Notes = a.Notes,
                Status = a.Status,
                CreatedAt = a.CreatedAt,
            }).ToListAsync();
            return View(appointments);
        }

        [Authorize(Roles = "Employee")]
        [HttpPost]
        //[Route("Home/reject/{id}")]
        public async Task<IActionResult> Approve(int? id)
        {
            if (id == null)
                return BadRequest();

            var appointment = await _context.Appointments.SingleOrDefaultAsync(a => a.Id == id);

            if (appointment == null)
                return NotFound();

            appointment.Status = "Approved";
            appointment.EmployeeId = Int32.Parse(User.Identity.GetUserId());

            _context.Appointments.Update(appointment);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [Authorize(Roles = "Employee")]
        [HttpPost]
        public async Task<IActionResult> Reject(int? id)
        {
            if (id == null)
                return BadRequest();

            var appointment = await _context.Appointments.SingleOrDefaultAsync(a => a.Id == id);

            if (appointment == null)
                return NotFound();

            appointment.Status = "Rejected";

            _context.Appointments.Update(appointment);
            await _context.SaveChangesAsync();

            return Ok();
        }
        
        
        [Authorize(Roles = "Employee")]
        [HttpPost]
        public async Task<IActionResult> Complete(int? id, string notes)
        {
            if (id == null)
                return BadRequest();

            var appointment = await _context.Appointments.SingleOrDefaultAsync(a => a.Id == id);

            if (appointment == null)
                return NotFound();

            appointment.Status = "Completed";
            appointment.Notes = notes;

            _context.Appointments.Update(appointment);
            await _context.SaveChangesAsync();

            return Ok();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
