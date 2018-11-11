using System;
using System.ComponentModel.DataAnnotations;

namespace AlbumsReviewRESTApi.Services.Data.DtoModels
{
    public class ArtistForCreationDto
    {
        public Guid Id { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Artist should have a stagename")]
        [StringLength(225, ErrorMessage = "Artist stagename should be less than 225 characters")]
        public string StageName { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Artist should have a first name")]
        [StringLength(225, ErrorMessage = "Artist first name should be less than 225 characters")]
        public string FirstName { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Artist should have a last name")]
        [StringLength(225, ErrorMessage = "Artist last name should be less than 225 characters")]
        public string LastName { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Artist should have a date of birth")]
        public DateTimeOffset DateOfBirth { get; set; }
    }
}
