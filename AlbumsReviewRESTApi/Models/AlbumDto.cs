using System;

namespace AlbumsReviewRESTApi.Models
{
    public class AlbumDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime Released { get; set; }
        public Guid ArtistId { get; set; }
    }
}
