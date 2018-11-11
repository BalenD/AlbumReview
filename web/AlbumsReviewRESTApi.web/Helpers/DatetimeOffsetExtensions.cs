using System;

namespace AlbumsReviewRESTApi.Web.Helpers
{
    public static class DatetimeOffsetExtensions
    {
        public static int CalculateAge(this DateTimeOffset datetimeOffset)
        {

            var currentDate = DateTime.UtcNow;


            int age = currentDate.Year - datetimeOffset.Year;

            return age;
        }
    }
}
