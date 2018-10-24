using System;

namespace AlbumsReviewRESTApi.Models
{
    public class ArtistDto
    {
        public Guid Id { get; set; }
        public string StageName { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
    }
}
