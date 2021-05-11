using System;

namespace API.Extensions
{
    public static class DateTimeExtension
    {
        public static int CalculateAge(this DateTime dob){
            return DateTime.Now.Year - dob.Year;
        }
    }
}