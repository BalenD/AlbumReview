using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AlbumsReviewRESTApi.Entities
{
    [Table("Albums")]
    public class Album
    {
        [Key]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public DateTimeOffset Released { get; set; }

        public Guid ArtistId { get; set; }

        public Artist Artist { get; set; }

        public IEnumerable<Review> Reviews { get; set; }

    }
}
