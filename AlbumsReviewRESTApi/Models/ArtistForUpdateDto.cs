using System;

namespace AlbumsReviewRESTApi.Models
{
    public class ArtistForUpdateDto
    {
        public string StageName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTimeOffset DateOfBirth { get; set; }
    }
}
