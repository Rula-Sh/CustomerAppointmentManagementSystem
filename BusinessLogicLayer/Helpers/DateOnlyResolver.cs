using AutoMapper;
using BusinessLogicLayer.DTOs;
using DataAccessLayer.Models;
using System.Globalization;

namespace BusinessLogicLayer.Helpers
{
    public class DateOnlyResolver : IValueResolver<DateTimeSlotGroupDTO, ServiceDate, DateOnly>
    {
        public DateOnly Resolve(DateTimeSlotGroupDTO source, ServiceDate destination, DateOnly destMember, ResolutionContext context)
        {
            return DateOnly.ParseExact(source.Date, "dd-MM-yyyy", CultureInfo.InvariantCulture);
        }
    }
}
