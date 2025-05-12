using CAMS.Data.Models;

namespace CAMS.Application.DTOs
{
    public class ServiceDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int EmployeeId { get; set; }
        public string Description { get; set; }
        public string Duration { get; set; }
        public decimal Price { get; set; }
        //public List<DateTime> AvailableTimeSlots { get; set; }

        //public List<string> SelectedDates { get; set; }
        //public Dictionary<string, List<string>> TimeSlots { get; set; }// Dictionary where key is the date string and value is a list of time strings
        public User Employee { get; set; }
        public List<DateTimeSlotGroupDTO> DateTimeSlotGroups { get; set; }
    }
}

//< div class= "form-group w-50" >
//                @* w-50 means taking 50% of the original width *@
//                < label class= "text-muted" asp -for= "Date" ></ label > @* it would add the Title by default *@
//                < input type = "text" class= "form-control" asp -for= "Date" value = "@(Model.Date > 0 ? Model.Date : string.Empty)" /> @* value="" to remove the default 0 value *@
//                < span asp - validation -for= "Date" class= "text-danger" ></ span >
//                @* to remove the arrows for inc and dec, i need to modify the site.css file*@
//            </ div >
//            < div class= "form-group" >
//                < label class= "text-muted" asp -for= "Time" ></ label > @* it would add the Title by default *@
//                < select class= "form-control" asp -for= "Time" asp - items = "@(new SelectList(Model.Time, "Id", "Name"))" >
//                    < option ></ option >
//                </ select >
//                < span asp - validation -for= "Time" class= "text-danger" ></ span >
//            </ div >
//            < div class= "form-group" >
//                < label class= "text-muted" asp -for= "AvailableFor" ></ label >
//                < input type = "text" class= "form-control" asp -for= "AvailableFor" placeholder = "10.00" />
//                < span asp - validation -for= "AvailableFor" class= "text-danger" ></ span >
//            </ div >