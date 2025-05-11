using AutoMapper;
using CAMS.Application.DTOs;
using CAMS.Web.ViewModels;

namespace CAMS.Web.Mapper
{
    public class PLAutoMapperProfile : Profile
    {
        public PLAutoMapperProfile()
        {
            // -------------------- GET --------------------

            CreateMap<UserDTO, UserViewModel>();

            CreateMap<ServiceDTO, ServiceViewModel>();
            CreateMap<DateTimeSlotGroupDTO, DateTimeSlotGroupViewModel>()
                .ForMember(dest => dest.Date, opt => opt.MapFrom<DateFormatResolver>());


            CreateMap<AppointmentDTO, AppointmentViewModel>();

            CreateMap<BookAppointmentDTO, BookAppointmentViewModel>();

            CreateMap<ActiveAppointmentDTO, ActiveAppointmentViewModel>();

            CreateMap<ServiceWithActiveAppointmentsDTO, ServiceWithActiveAppointmentsViewModel>();

            // -------------------- POST --------------------
            // create a service
            CreateMap<ServiceViewModel, ServiceDTO>();
            CreateMap<DateTimeSlotGroupViewModel, DateTimeSlotGroupDTO>();
            //create an appointment
            CreateMap<BookAppointmentViewModel, BookAppointmentDTO>();

        }
    }
}
