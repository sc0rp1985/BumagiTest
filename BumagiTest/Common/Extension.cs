using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class ExtentsionIEnumerable
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> collection)
        {
            return (collection == null) || !collection.Any();
        }
    }

    public static class ExtensionString
    {
        public static int? AsInt(this string str)
        {
            int n;
            return Int32.TryParse(str, out n) ? n : (int?) null;
        }

        public static DateTime? AsDate(this string str)
        {
            DateTime dt;
            if(DateTime.TryParseExact(
                str.Trim(),
                "dd.MM.yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out dt))
                    return dt;
            else
            {
                return (DateTime?)null;
            }
            
        }
    }
}
