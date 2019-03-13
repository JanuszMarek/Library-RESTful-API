using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library_RESTful_API.Helpers
{
    public static class DateTimeOffsetExtension
    {
        public static int GetCurentAge(this DateTimeOffset dateTimeOffset)
        {
            var currentDate = DateTime.UtcNow;
            int age = currentDate.Year - dateTimeOffset.Year;

            if(currentDate < dateTimeOffset.AddYears(age))
            {
                age--;
            }

            return age;
        }
    }
}
