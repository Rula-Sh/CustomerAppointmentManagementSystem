using AutoMapper;
using CAMS.Application.DTOs;
using CAMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CAMS.Web.ViewModels;

namespace PresentationLayer.Controllers
{
    [Authorize(Roles = "Admin, Provider")]

    public class ServicesController : Controller
    {
        private readonly IManageUsersService _manageUsers;
        private readonly IManageServicesService _manageServices;
        private readonly IMapper _mapper;


        public ServicesController(IManageUsersService manageUsers, IManageServicesService manageServices, IMapper mapper)
        {
            _manageUsers = manageUsers;
            _manageServices = manageServices;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            await _manageUsers.UpdateUserLastActivityDate(User);

            var servicesDTO = await _manageServices.GetProviderServices(User);
            if (User.IsInRole("Admin"))
            {
                servicesDTO = await _manageServices.GetAllServices();
            }

            var services = _mapper.Map<List<ServiceViewModel>>(servicesDTO);

            return View(services);
        }

        public async Task<IActionResult> Add()
        {
            await _manageUsers.UpdateUserLastActivityDate(User);

            var viewModel = new ServiceViewModel { ProviderId = int.Parse(_manageUsers.GetUserId(User)) };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Add(ServiceViewModel model)
        {
            // make sure at least one time-slot is selected for each selected Date
            if (model.DateTimeSlotGroups == null || !model.DateTimeSlotGroups.Any(g => g.TimeSlots != null && g.TimeSlots.Any()))
            {
                // g here is each date group => { '11-05-2025': ['8:00'-'9:30','9:30'-'11:00',...], ...}
                // g.TimeSlots != null ensures the list is not null.
                // g.TimeSlots.Any() checks that the list contains at least one time-slot string.
                ModelState.AddModelError("", "At least one time slot for one date is required.");
                return View(model);
            }

            ModelState.Remove(nameof(model.Provider));
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var service = _mapper.Map<ServiceDTO>(model);

            await _manageServices.addService(service, User);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> CanEditOrDelete(int? id)
        {
            if (!await _manageServices.DoesTheServiceHaveAppointments(id))
            {
                return Ok(new { success = true });
            }
            else
            {
                return Ok(new { success = false, message = "Cannot Edit or Delete a Service with Active Appointments." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            await _manageUsers.UpdateUserLastActivityDate(User);

            var serviceDTO = await _manageServices.getServiceById(id);

            var serviceViewModel = _mapper.Map<ServiceViewModel>(serviceDTO);

            return View(serviceViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ServiceViewModel model)
        {
            if (model.DateTimeSlotGroups == null || !model.DateTimeSlotGroups.Any(g => g.TimeSlots != null && g.TimeSlots.Any()))
            {
                ModelState.AddModelError("", "At least one time slot for one date is required.");
                return View(model);
            }

            ModelState.Remove(nameof(model.Provider));
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var service = _mapper.Map<ServiceDTO>(model);


            await _manageServices.updateService(service, User);

            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Details(int? id)
        {
            await _manageUsers.UpdateUserLastActivityDate(User);

            if (id == null)
                return BadRequest();

            var ServiceWithActiveAppointmentsDTO = await _manageServices.getServiceWithActiveAppointments(id);

            if (ServiceWithActiveAppointmentsDTO == null)
                return NotFound();

            var service = _mapper.Map<ServiceWithActiveAppointmentsViewModel>(ServiceWithActiveAppointmentsDTO);

            return View("Details", service);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int? id)
        {
            await _manageUsers.UpdateUserLastActivityDate(User);

            if (id == null)
                return BadRequest();

            var service = await _manageServices.getServiceById(id);

            if (service == null)
                return NotFound();

            if (!await _manageServices.DoesTheServiceHaveAppointments(id))
            {
                await _manageServices.DeleteService(service, User);

                return Ok(new { success = true, message = "Service Deleted." });
            }
            else
            {
                return Ok(new { success = false, message = "Cannot Delete a Service with Active Appointments." });
            }

        }
    }
}
