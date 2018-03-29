using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UXR.Studies.Files
{
    public static class Formats
    {
        public const string DateTimeFormat = "yyyy'-'MM'-'dd'T'HH'_'mm'_'ss"; //-'fffffffK";  // misses time zones with 30 and 45 min offset

        public static string ConvertToFilePathValidString(DateTime dateTime)
        {
            return dateTime.ToString(DateTimeFormat);
        }

        public static bool TryConvertFromFilePathString(string dateTimeString, out DateTime dateTime)
        {                                                                                                           
            return DateTime.TryParseExact(dateTimeString, DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out dateTime);
        }
    }
}
