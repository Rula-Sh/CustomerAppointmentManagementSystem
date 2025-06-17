﻿using AutoMapper;
using CAMS.Application.Interfaces;
using CAMS.Data;
using CAMS.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CAMS.Web.ViewModels;
using System.Globalization;

namespace PresentationLayer.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly IManageUsersService _manageUsers;
        private readonly IManageServicesService _manageServices;
        private readonly IManageAppointmentsService _manageAppointments;
        private readonly IMapper _mapper;
        const string usersPath = "~/Views/Admin/Dashboard/Index.cshtml";

        public DashboardController(IManageUsersService manageUsers, IManageServicesService manageServices, IManageAppointmentsService manageAppointments, IMapper mapper)
        {
            _manageUsers = manageUsers;
            _manageServices = manageServices;
            _manageAppointments = manageAppointments;
            _mapper = mapper;
        }
        public async Task<IActionResult> Index()
        {

            //Best Provider
            var bestProviderName = _manageAppointments.getProviderNameWithMostCompletedAppointments();


            //Average Appointments per Provider
            double avgAppointmentsPerProvider = _manageAppointments.GetAverageAppointmentsPerProvider();


            //Most Booked Service
            var mostBookedServiceName = _manageServices.GetMostBookedServiceName();


            // --------------------- Daily Booking ---------------------

            // set the last 7 days format to the days of the week
            var last7DaysLabels = _manageAppointments.getLast7Days();
            // count approved appointments per day
            var dailyCounts = await _manageAppointments.getTotalAppointmentsFromPast7Days();


            // --------------------- Weekly Booking (4 weeks) ---------------------

            var weeklyLabels = _manageAppointments.getLast4WeeksDates().Select(d => d.ToString("MMM dd")).ToList();
            var weeklyCounts = _manageAppointments.getTotalApprovedAppointemntPerWeek();


            // --------------------- Appointments Status ---------------------
            var appointmentsStatus = _manageAppointments.GetAppointmentsStatusCount();

            // --------------------- Appointments Per Service ---------------------
            var servicesLabel = await _manageServices.GetServicesNames();
            var serviceAppointmentsCount = await _manageAppointments.getTotalAppointmentsPerService();

            // --------------------- Active Customers ---------------------

            var activeAppointmentsDTOs = await _manageAppointments.getTodaysAppointments();
            var activeAppointments = _mapper.Map<List<ActiveAppointmentViewModel>>(activeAppointmentsDTOs);

            var model = new DashboardViewModel
            {
                TotalUsers = _manageUsers.GetTotalUsers(),
                TotalServices = _manageServices.GetTotalServices(),
                TotalAppointments = _manageAppointments.GetTotalActiveAndCompletedAppointments(),

                BestProvider = bestProviderName,
                AvgAppointmentsPerProvider = avgAppointmentsPerProvider,
                MostBookedService = mostBookedServiceName,

                last7Days = last7DaysLabels,
                dailyBookingCounts = dailyCounts,

                lastWeeksinMonth = weeklyLabels,
                weeklyBookingCounts = weeklyCounts,

                appointmentsStatusCount = appointmentsStatus,

                servicesLabel = servicesLabel,
                serviceAppointmentsCount = serviceAppointmentsCount,

                ActiveAppointments = activeAppointments,
            };

            return View(usersPath, model);
        }
    }
}
