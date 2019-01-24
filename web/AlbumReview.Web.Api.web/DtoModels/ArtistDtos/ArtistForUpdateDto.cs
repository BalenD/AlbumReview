using System;
using System.ComponentModel.DataAnnotations;

namespace AlbumReview.Web.Api.DtoModels
{
    public class ArtistForUpdateDto
    {
        [StringLength(maximumLength: 225, ErrorMessage = "Artist stagename should be less than 225 characters")]
        public string StageName { get; set; }
        [StringLength(maximumLength: 225, ErrorMessage = "Artist first name should be less than 225 characters")]
        public string FirstName { get; set; }
        [StringLength(maximumLength: 225, ErrorMessage = "Artist last name should be less than 225 characters")]
        public string LastName { get; set; }
        public DateTimeOffset DateOfBirth { get; set; }
    }
}
