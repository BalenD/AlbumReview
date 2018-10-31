using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AlbumsReviewRESTApi.Entities
{
    [Table("Albums")]
    public class Album
    {
        public Album()
        {
            Reviews = new List<Review>();
        }
        [Key]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public DateTimeOffset Released { get; set; }

        public Guid ArtistId { get; set; }

        public Artist Artist { get; set; }

        public ICollection<Review> Reviews { get; set; }

    }
}
