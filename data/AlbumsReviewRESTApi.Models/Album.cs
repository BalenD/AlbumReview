using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlbumsReviewRESTApi.Data.Models
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

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        public DateTimeOffset Released { get; set; }

        [Required]
        public Guid ArtistId { get; set; }

        public Artist Artist { get; set; }

        public ICollection<Review> Reviews { get; set; }
    }
}
