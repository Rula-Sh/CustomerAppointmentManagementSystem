using AutoMapper;
using BusinessLogicLayer.DTOs;
using BusinessLogicLayer.Interfaces;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.ViewModels;

namespace PresentationLayer.Controllers
{
    [Authorize(Roles = "Admin, Employee")]

    public class ServicesController : Controller
    {
        private readonly IManageUsersService _manageUsers;
        private readonly IManageServicesService _manageServices;
        private readonly IMapper _mapper;


        public ServicesController(IManageUsersService manageUsers,IManageServicesService manageServices, IMapper mapper)
        {
            _manageUsers = manageUsers;
            _manageServices = manageServices;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            await _manageUsers.UpdateUserLastActivityDate(User);

            /*var posts = await _context.Posts.OrderByDescending(m => m.DatePublished.Year).ToListAsync(); // this will get me the list of posts*/
            //var services = await _context.Services.ToListAsync(); // this will get me the list of services

            //OrderByDescending(m => m.DatePublished.Year).ToListAsync() will order the DatePublished of the posts from the top to the bottom
            //return View(services);

            var servicesDTO = await _manageServices.GetAllServices();
            var services = _mapper.Map<List<ServiceViewModel>>(servicesDTO);

            return View(services);
        }

        public async Task<IActionResult> Add()
        {
            await _manageUsers.UpdateUserLastActivityDate(User);

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

            var service = _mapper.Map<ServiceDTO>(model);
            /*// create Service
            *//*var service = new Service
            {
                Name = model.Name,
                Description = model.Description,
                Duration = model.Duration,
                Price = model.Price,
                ServiceDates = new List<ServiceDate>()
            };*//*
            // using AutoMapper
            var service = _mapper.Map<Service>(model);

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
                *//*var serviceDate = new ServiceDate
                {
                    ServiceId = service.Id, // to link it to Services Table
                    Date = date,
                    ServiceTimeSlots = new List<ServiceTimeSlot>()
                };*//*
                // using AutoMapper
                var serviceDate = _mapper.Map<ServiceDate>(group);
                serviceDate.Date = date;

                foreach (var time in group.TimeSlots)
                {
                    // create ServiceTimeSlot
                    *//*serviceDate.ServiceTimeSlots.Add(new ServiceTimeSlot
                    {
                        ServiceDateId = serviceDate.Id, // to link it to ServiceDate Table
                        Time = time
                    });*//*
                    // using AutoMapper
                    var serviceTimeSlot = new ServiceTimeSlot
                    {
                        ServiceDateId = serviceDate.Id, // to link it to ServiceDate Table
                        Time = time
                    };

                    serviceDate.ServiceTimeSlots.Add(serviceTimeSlot);  // add it to ServiceDate Table

                }

                service.ServiceDates.Add(serviceDate); // add ServiceDate list to Services Table
            }*/

            // update DB
            await _manageServices.addService(service,User);

            return RedirectToAction("Index");
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

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var service = _mapper.Map<ServiceDTO>(model);


            // update DB
            await _manageServices.updateService(service, User);

            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Details(int? id)
        {
            await _manageUsers.UpdateUserLastActivityDate(User);

            if (id == null)
                return BadRequest();

            var serviceDTO = await _manageServices.getService(id);

            if (serviceDTO == null)
                return NotFound();

            var service = _mapper.Map<ServiceViewModel>(serviceDTO);

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
                await _manageServices.DeleteService(service,User);

                return Ok(new { success = true, message = "Service Deleted Successfully." });
            }
            else
            {
                return Ok(new { success = false, message = "Cannot Delete a Service with Active Appointments." });
            }

        }
    }
}
