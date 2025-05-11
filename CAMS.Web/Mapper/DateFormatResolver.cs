using System.Globalization;
using AutoMapper;
using CAMS.Application.DTOs;
using CAMS.Web.ViewModels;

namespace CAMS.Web.Mapper
{
    public class DateFormatResolver : IValueResolver<DateTimeSlotGroupDTO, DateTimeSlotGroupViewModel, string>
    {
        public string Resolve(DateTimeSlotGroupDTO source, DateTimeSlotGroupViewModel destination, string destMember, ResolutionContext context)
        {
            if (DateTime.TryParseExact(source.Date, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            {
                return date.ToString("dd-MM-yyyy");
            }
            return source.Date; // or null / empty .. idk check first what is best
        }
    }
}
