﻿using AutoMapper;
using BusinessLogicLayer.DTOs;
using DataAccessLayer.Models;
using System.Globalization;

namespace BusinessLogicLayer.Helpers
{
    public class BLLAutoMapperProfile : Profile
    {
        public BLLAutoMapperProfile()
        {
            // -------------------- GET --------------------
            CreateMap<User, UserDTO>()
                .ForMember(dest => dest.LastActivity, opt => opt.MapFrom(src => TimeDifferenceHelper.getTimeDifference(src.LastActivityDate)))
                .ForMember(dest => dest.Roles, opt => opt.Ignore())
                .ReverseMap();
                //Why did ignore Roles? Because getRoles() in the UsersController is asynchronous, and AutoMapper doesn't support asynchronous value resolvers out of the box. So I'll still have to set Roles manually after mapping.

            CreateMap<Service, ServiceDTO>()
                .ForMember(dest => dest.DateTimeSlotGroups, opt => opt.MapFrom(src => src.ServiceDates));
            CreateMap<ServiceDate, DateTimeSlotGroupDTO>()
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)))
                .ForMember(dest => dest.TimeSlots, opt => opt.MapFrom(src => src.ServiceTimeSlots.Select(t => t.Time).ToList()));

            CreateMap<Appointment, AppointmentDTO>();

            CreateMap<Service, BookAppointmentDTO>();

            CreateMap<Appointment, ActiveAppointmentDTO>()
            .ForMember(dest => dest.CustomerName,opt => opt.MapFrom(src => src.Customer.FullName)) 
            .ForMember(dest => dest.AppointmentDate, opt => opt.MapFrom(src => src.Date))
            .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.Service.Name));

            // -------------------- POST --------------------
            // create a service
            CreateMap<ServiceDTO, Service >()
                .ForMember(dest => dest.ServiceDates, opt => opt.MapFrom(src => src.DateTimeSlotGroups));
            CreateMap<DateTimeSlotGroupDTO, ServiceDate>()
                .ForMember(dest => dest.Date, opt => opt.MapFrom<DateOnlyResolver>())
                .ForMember(dest => dest.ServiceTimeSlots, opt => opt.MapFrom(src => src.TimeSlots.Select(t => new ServiceTimeSlot { Time = t }).ToList()));
            //create an appointment
            CreateMap<BookAppointmentDTO, Appointment>()
            .ForMember(dest => dest.CustomerId, opt => opt.Ignore())
            .ForMember(dest => dest.EmployeeId, opt => opt.Ignore())
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ServiceName))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => "Pending"))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.Now))
            .ForMember(dest => dest.Notes, opt => opt.MapFrom(_ => ""));


            CreateMap<AppointmentDTO, Appointment>();


            /*// -------------------- GET --------------------
            CreateMap<User, UserDTO>()
                .ForMember(dest => dest.LastActivity, opt => opt.MapFrom(src => TimeDifferenceHelper.getTimeDifference(src.LastActivityDate)))
                .ForMember(dest => dest.Roles, opt => opt.Ignore());
            //Why did ignore Roles? Because getRoles() in the UsersController is asynchronous, and AutoMapper doesn't support asynchronous value resolvers out of the box. So I'll still have to set Roles manually after mapping.

            CreateMap<Service, ServiceDTO>()
                .ForMember(dest => dest.DateTimeSlotGroups, opt => opt.MapFrom(src => src.ServiceDates));
            CreateMap<ServiceDate, DateTimeSlotGroupDTO>()
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date.ToString()))
                .ForMember(dest => dest.TimeSlots, opt => opt.MapFrom(src => src.ServiceTimeSlots.Select(t => t.Time).ToList()));

            CreateMap<Appointment, AppointmentDTO>();

            CreateMap<Service, BookAppointmentDTO>();

            CreateMap<Appointment, ActiveAppointmentDTO>()
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.FullName))
            .ForMember(dest => dest.AppointmentDate, opt => opt.MapFrom(src => src.Date))
            .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.Service.Name));

            // -------------------- POST --------------------
            // create a service
            CreateMap<ServiceDTO, Service>()
                .ForMember(dest => dest.ServiceDates, opt => opt.MapFrom(src => src.DateTimeSlotGroups));
            CreateMap<DateTimeSlotGroupDTO, ServiceDate>()
                .ForMember(dest => dest.Date, opt => opt.MapFrom<DateOnlyResolver>())
                .ForMember(dest => dest.ServiceTimeSlots, opt => opt.MapFrom(src => src.TimeSlots.Select(t => new ServiceTimeSlot { Time = t }).ToList()));
            //create an appointment
            CreateMap<BookAppointmentDTO, Appointment>()
            .ForMember(dest => dest.CustomerId, opt => opt.Ignore())
            .ForMember(dest => dest.EmployeeId, opt => opt.Ignore())
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ServiceName))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => "Pending"))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.Now))
            .ForMember(dest => dest.Notes, opt => opt.MapFrom(_ => ""));*/

        }
    }
}
