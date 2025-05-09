using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//https://rmauro.dev/calculate-time-ago-with-csharp/

namespace CAMS.Application.Helpers
{
    public class TimeDifferenceHelper
    {
        const int SECOND = 1;
        const int MINUTE = 60 * SECOND;
        const int HOUR = 60 * MINUTE;
        const int DAY = 24 * HOUR;
        const int MONTH = 30 * DAY;

        public static string getTimeDifference(DateTime lastDate)
        {
            var td = new TimeSpan(DateTime.Now.Ticks - lastDate.Ticks);
            double delta = Math.Abs(td.TotalSeconds);

            if (delta < 1 * MINUTE)
                return td.Seconds == 1 ? "one second ago" : td.Seconds + " seconds ago";

            if (delta < 2 * MINUTE)
                return "a minute ago";

            if (delta < 45 * MINUTE)
                return td.Minutes + " minutes ago";

            if (delta < 90 * MINUTE)
                return "an hour ago";

            if (delta < 24 * HOUR)
                return td.Hours + " hours ago";

            if (delta < 48 * HOUR)
                return "yesterday";

            if (delta < 30 * DAY)
                return td.Days + " days ago";

            if (delta < 12 * MONTH)
            {
                int months = Convert.ToInt32(Math.Floor((double)td.Days / 30));
                return months <= 1 ? "one month ago" : months + " months ago";
            }
            else
            {
                int years = Convert.ToInt32(Math.Floor((double)td.Days / 365));
                return years <= 1 ? "one year ago" : years + " years ago";
            }
        }
    }
}
