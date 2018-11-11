using System;
using System.ComponentModel.DataAnnotations;

namespace AlbumsReviewRESTApi.Services.Data.DtoModels
{
    public class ReviewDto
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        //  TODO add proper charascter annotation
        public string SubmittedReview { get; set; }
        [Required]
        [Range(0, 10)]
        public float Rating { get; set; }
        [Required]
        public DateTimeOffset Submitted { get; set; }
        public Guid AlbumId { get; set; }
    }
}
