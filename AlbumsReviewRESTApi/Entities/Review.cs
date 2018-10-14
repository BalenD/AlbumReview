using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlbumsReviewRESTApi.Entities
{
    [Table("Reviews")]
    public class Review
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string SubmittedReview { get; set; }

        [Required]
        [Range(0, 10)]
        public float Rating { get; set; }

        public DateTimeOffset Submitted { get; set; }

        public Guid AlbumId { get; set; }

        public Album Album { get; set; }

        //  TODO:
        //  the creator of the review propery


    }
}
