using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AlbumReview.Data.Common;

namespace AlbumReview.Data.Models
{
    [Table("Albums")]
    public class Album : BaseEntityModel<Guid>
    {
        public Album()
        {
            Reviews = new List<Review>();
        }

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
