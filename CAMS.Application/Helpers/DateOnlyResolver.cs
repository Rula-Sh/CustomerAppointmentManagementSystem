using AutoMapper;
using CAMS.Application.DTOs;
using CAMS.Data.Models;
using System.Globalization;

namespace CAMS.Application.Helpers
{
    public class DateOnlyResolver : IValueResolver<DateTimeSlotGroupDTO, ServiceDate, DateOnly>
    {
        public DateOnly Resolve(DateTimeSlotGroupDTO source, ServiceDate destination, DateOnly destMember, ResolutionContext context)
        {
            return DateOnly.ParseExact(source.Date, "dd-MM-yyyy", CultureInfo.InvariantCulture);
        }
    }
}
