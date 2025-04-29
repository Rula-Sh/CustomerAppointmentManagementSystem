using AutoMapper;
using DataAccessLayer.Models.ViewModel;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
