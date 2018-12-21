using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AlbumReview.Data.Common;

namespace AlbumReview.Data.Models
{
    [Table("Reviews")]
    public class Review : DeleteableEntityModel<Guid>
    {

        [Required]
        public string SubmittedReview { get; set; }

        [Required]
        [Range(0, 10)]
        public float Rating { get; set; }
        [Required]
        public DateTimeOffset Submitted { get; set; }
        [Required]
        public Guid AlbumId { get; set; }

        public Album Album { get; set; }
    }
}
