using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCommLib.Classes
{
    public class clsLocalTime
    {
        public static DateTime Now() => Now("SYD");
        //public static DateTime GetNow(string city) => TimeZoneInfo.ConvertTime(DateTime.Now, tz(city));
        public static DateTime Now(string city) => TimeZoneInfo.ConvertTime(DateTime.Now, tz(city));
        public static DateTime Today() => Today("SYD");
        public static DateTime Today(string city) => TimeZoneInfo.ConvertTime(DateTime.Now, tz(city)).Date;
        public static DateTime ToUtc(string city, DateTime time) => TimeZoneInfo.ConvertTimeToUtc(time, tz(city));
        public static DateTime FromUtc(string city, DateTime utc) => TimeZoneInfo.ConvertTimeFromUtc(utc, tz(city));
        public static TimeZoneInfo tz() => tz("SYD");
        public static TimeZoneInfo tz(string code)
        {
            var tz = GetTimeZoneInfo(code, LocalCities);
            if (tz is null)
            {
                tz = GetTimeZoneInfo(code, LocalCities2);
            }
            return tz;
        }
        private static TimeZoneInfo GetTimeZoneInfo(string code, List<LocalCity> localCities)
        {
            foreach (var LocalCity in localCities)
            {
                if (LocalCity.Code == code)
                {
                    try
                    {
                        return TimeZoneInfo.FindSystemTimeZoneById(LocalCity.TimeZoneId);
                    }
                    catch { }
                }
            }
            return null!;
        }
        public static bool IsDS(string code) => tz(code).IsDaylightSavingTime(Now(code));

        public static List<LocalCity> LocalCities = new List<LocalCity>() 
        {
            new LocalCity { Code = "UTC", TimeZoneId = "UTC" },
            new LocalCity { Code = "SYD", TimeZoneId = "Australia/Sydney" },
            new LocalCity { Code = "QLD", TimeZoneId = "Australia/Brisbane" },
            new LocalCity { Code = "VIC", TimeZoneId = "Australia/Melbourne" },
            new LocalCity { Code = "WA", TimeZoneId = "Australia/Perth" },
            new LocalCity { Code = "SA", TimeZoneId = "Australia/Adelaide" },
            new LocalCity { Code = "TAS", TimeZoneId = "Australia/Hobart" },
            new LocalCity { Code = "NT", TimeZoneId = "Australia/Darwin" },
            new LocalCity { Code = "TKO", TimeZoneId = "Asia/Tokyo" },
            new LocalCity { Code = "NZ", TimeZoneId = "Pacific/Auckland" },
        };
        public static List<LocalCity> LocalCities2 = new List<LocalCity>()
        {
            new LocalCity { Code = "UTC", TimeZoneId = "UTC" },
            new LocalCity { Code = "SYD", TimeZoneId = "AUS Eastern Standard Time" },
            new LocalCity { Code = "QLD", TimeZoneId = "E. Australia Standard Time" },
            new LocalCity { Code = "VIC", TimeZoneId = "AUS Eastern Standard Time" },
            new LocalCity { Code = "WA", TimeZoneId = "W. Australia Standard Time" },
            new LocalCity { Code = "SA", TimeZoneId = "Cen. Australia Standard Time" },
            new LocalCity { Code = "TAS", TimeZoneId = "Tasmania Standard Time" },
            new LocalCity { Code = "NT", TimeZoneId = "Aus Central Standard Time" },
            new LocalCity { Code = "TKO", TimeZoneId = "Tokyo Standard Time" }
        };

        public class LocalCity
        {
            public string Code { get; set; } = default!;
            public string TimeZoneId { get; set; } = default!;
        }
    }
}
