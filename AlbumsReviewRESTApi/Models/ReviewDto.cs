using System;
using System.ComponentModel.DataAnnotations;

namespace AlbumsReviewRESTApi.Models
{
    public class ReviewDto
    {
        public Guid Id { get; set; }
        [Required]
        public string SubmittedReview { get; set; }
        [Required]
        [Range(0, 10)]
        public float Rating { get; set; }
        public DateTimeOffset Submitted { get; set; }
        public Guid AlbumId { get; set; }
    }
}
