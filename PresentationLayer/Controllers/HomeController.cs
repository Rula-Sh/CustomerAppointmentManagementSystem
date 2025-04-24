using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.Services;
using DataAccessLayer.Data;
using DataAccessLayer.Models;
using DataAccessLayer.Models.ViewModel;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Diagnostics;


namespace PresentationLayer.Controllers
{
    public class HomeController : Controller
    {
        private readonly IManageUsers _manageUsers;
        private readonly IManageServices _manageServices;
        private readonly IManageAppointments _manageAppointments;

        public HomeController(IManageUsers manageUsers,
                              IManageServices manageServices,
                              IManageAppointments manageappointments)
        {
            _manageUsers= manageUsers;
            _manageServices = manageServices;
            _manageAppointments = manageappointments;
        }

        public async Task<IActionResult> Index()
        {

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            await _manageUsers.UpdateUserLastActivityDate(User);

            var appointments = await _manageAppointments.getAppointmentsBasedOnRole(User);

            return View(appointments);
        }

        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Add()
        {
            await _manageUsers.UpdateUserLastActivityDate(User);

            var viewModel = await _manageAppointments.ViewAddAppointment();

            return View(viewModel); // had to pass a viewModel so that i dont get an error in Add.cshtml where it expects PostFormViewModel object (System.NullReferenceException: 'Object reference not set to an instance of an object.'... Microsoft.AspNetCore.Mvc.Razor.RazorPage<TModel>.Model.get returned null.)

        }

        [HttpGet]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetServiceDetails(int serviceId)
        {
            await _manageUsers.UpdateUserLastActivityDate(User);

            var service = await _manageServices.getService(serviceId);

            if (service == null)
                return NotFound();

            var serviceViewModel = _manageServices.getSelectedServiceDetails(service);

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

            await _manageAppointments.addAppointment(model, User);

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Employee, Customer")]
        public async Task<IActionResult> Details(int? id)
        {
            await _manageUsers.UpdateUserLastActivityDate(User);

            if (id == null)
                return BadRequest();

            var appointment = await _manageAppointments.appointmentDetails(id);

            if (appointment == null)
                return NotFound();

            return View("Details", appointment);
        }

        [Authorize(Roles ="Customer")]
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

            var appointments = await _manageAppointments.getPendingAppointments(User);

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
            appointment.EmployeeId = Int32.Parse(User.Identity.GetUserId());
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
