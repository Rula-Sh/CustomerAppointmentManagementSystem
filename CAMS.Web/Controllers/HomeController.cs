using System.Diagnostics;
using AutoMapper;
using CAMS.Application.DTOs;
using CAMS.Application.Interfaces;
using CAMS.Data.Models;
using CAMS.Data;
using CAMS.Web.Models;
using CAMS.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CAMS.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IManageUsersService _manageUsers;
        private readonly IManageServicesService _manageServices;
        private readonly IManageAppointmentsService _manageAppointments;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        public HomeController(IManageUsersService manageUsers,
                              IManageServicesService manageServices,
                              IManageAppointmentsService manageappointments,
                              IMapper mapper,
                              UserManager<User> userManager)
        {
            _manageUsers = manageUsers;
            _manageServices = manageServices;
            _manageAppointments = manageappointments;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            await _manageUsers.UpdateUserLastActivityDate(User);

            var appointmentsDTOs = await _manageAppointments.getAppointmentsBasedOnRole(User);
            var appointments = _mapper.Map<List<AppointmentViewModel>>(appointmentsDTOs);

            return View(appointments);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> LoadCustomerAppointments()
        {
            using (ApplicationDbContext appDBC = new ApplicationDbContext())
            {
                var appointments = await _manageAppointments.getAppointmentsBasedOnRole(User);
                var tableData = appointments.Select(a => new {
                    a.Id,
                    a.Name,
                    Date = a.Date.ToString(),
                    a.Status
                });

                return Json(new { data = tableData });
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> LoadEmployeeAppointments()
        {
            using (ApplicationDbContext appDBC = new ApplicationDbContext())
            {
                var appointments = await _manageAppointments.getAppointmentsBasedOnRole(User);
                var tableData = appointments.Select(a => new {
                    //a.Id,
                    a.Name,
                    CustomerFullName = a.Customer.FullName,
                    Date = a.Date.ToString(),
                });

                return Json(new { data = tableData });
            }
        }

        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Add()
        {
            await _manageUsers.UpdateUserLastActivityDate(User);

            //var appointmentDTO = await _manageAppointments.ViewAddAppointment();

            // Note: ManageAppointmentsService was calling IManageServices.GetServices(), which caused: ManageAppointmentsService ? ManageServices ? ManageAppointmentsService (circular reference).
            // to fix this, I moved ViewAddAppointment code here (in home controller) to keep the business logic layers independent and preventing InvalidOperationException from DI container.
            // this svoided circular dependency between ManageAppointmentsService and ManageServices

            //Circular Dependency: occurs when two or more components (classes, modules, or services) depend on each other either directly or indirectly, creating a loop in the dependency chain. This can lead to issues like:
            // 1- Stack overflow or infinite recursion.
            // 2- Difficulty in managing dependencies and testing.
            // 3- Problems during dependency injection, especially with frameworks like ASP.NET Core's built-in DI container.
            var services = await _manageServices.GetAvailableServicesInAddAppointment(User);
            var appointmentDTO = new BookAppointmentDTO
            {
                Services = services,
            };
            // i  don’t need AutoMapper here because I'm not really mapping anything — just assigning a list to a property. 

            var bookAppointmentViewModel = _mapper.Map<BookAppointmentViewModel>(appointmentDTO);

            return View(bookAppointmentViewModel); // had to pass a viewModel so that i dont get an error in Add.cshtml where it expects BookAppointmentViewModel object (System.NullReferenceException: 'Object reference not set to an instance of an object.'... Microsoft.AspNetCore.Mvc.Razor.RazorPage<TModel>.Model.get returned null.)

        }

        [HttpGet]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetServiceDetails(int serviceId)
        {
            await _manageUsers.UpdateUserLastActivityDate(User);

            var service = await _manageServices.getService(serviceId);
            var serviceViewModel = _mapper.Map<ServiceViewModel>(service);

            if (service == null)
                return NotFound();

            //var serviceViewModel = _manageServices.getSelectedServiceDetails(service);

            return Json(serviceViewModel);
            // this returns a the serviceViewModel as a JSON object... wraps it in an HTTP response... then sends it to the client browser
            // from the client side: i gets the JSON-formatted string and  convert it again to JavaScript object
            // summary: C# Object ? JSON string ? HTTP ? JSON string ? JavaScript Object
        }

        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Add(BookAppointmentViewModel model)
        {
            ModelState.Remove("Services"); // i no longer need it, i got the selected appointment date
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var bookAppointmentDTO = _mapper.Map<BookAppointmentDTO>(model);

            await _manageAppointments.addAppointment(bookAppointmentDTO, User);

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Employee, Customer")]
        public async Task<IActionResult> Details(int? id)
        {
            await _manageUsers.UpdateUserLastActivityDate(User);

            if (id == null)
                return BadRequest();

            var appointmentDTO = await _manageAppointments.appointmentDetails(id);

            if (appointmentDTO == null)
                return NotFound();

            var appointment = _mapper.Map<AppointmentViewModel>(appointmentDTO);

            return View("Details", appointment);
        }

        [Authorize(Roles = "Customer")]
        [HttpDelete]
        public async Task<IActionResult> Delete(int? id)
        {
            await _manageUsers.UpdateUserLastActivityDate(User);

            if (id == null)
                return BadRequest();

            var appointment = await _manageAppointments.getAppointmentById(id);

            if (appointment == null)
                return NotFound();

            await _manageAppointments.deleteAppointment(appointment);

            return Ok();
        }


        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> PendingAppointments()
        {
            await _manageUsers.UpdateUserLastActivityDate(User);

            var appointmentsDTO = await _manageAppointments.getPendingAppointments(User);
            var appointments = _mapper.Map<List<AppointmentViewModel>>(appointmentsDTO);

            return View(appointments);
        }

        [Authorize(Roles = "Employee")]
        [HttpPost]
        [Route("home/approve")]
        //[Route("Home/Approve/{id}")]
        public async Task<IActionResult> Approve(int? id, string notes)
        {
            if (id == null)
                return BadRequest();

            var appointment = await _manageAppointments.getAppointmentById(id);

            if (appointment == null)
                return NotFound();

            appointment.Status = "Approved";
            appointment.EmployeeId = int.Parse(_userManager.GetUserId(User));
            //was: appointment.EmployeeId = int.Parse(User.Identity.GetUserId()); before removing the import "using Microsoft.AspNet.Identity;"
            appointment.Notes = notes;
            //no need to use a n automepper here, because i am getting the appointment to update it internally. I am not returning it to the view or exposing it externally — so there's no real need to map it to a ViewModel:
            // i should use the automapperin this case if: i want to show it to the user (like for approval or a details page) / i want to enforce separation of concerns more strictly

            await _manageAppointments.updateAppointment(appointment);

            return Ok();
        }

        [Authorize(Roles = "Employee")]
        [HttpPost]
        //[Route("home/reject")]
        public async Task<IActionResult> Reject(int? id)
        {
            if (id == null)
                return BadRequest();

            var appointment = await _manageAppointments.getAppointmentById(id);

            if (appointment == null)
                return NotFound();

            appointment.Status = "Rejected";

            await _manageAppointments.updateAppointment(appointment);

            return Ok();
        }


        [Authorize(Roles = "Employee")]
        [HttpPost]
        [Route("complete")]
        public async Task<IActionResult> Complete(int? id, string notes)
        {
            if (id == null)
                return BadRequest();

            var appointment = await _manageAppointments.getAppointmentById(id);

            if (appointment == null)
                return NotFound();

            appointment.Status = "Completed";
            appointment.Notes = notes;

            await _manageAppointments.updateAppointment(appointment);

            return Ok();
        }


        public IActionResult About()
        {
            return View();
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
