using System;
using System.ComponentModel.DataAnnotations;

namespace AlbumReview.Services.Data.DtoModels
{
    public class ArtistDto
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Artist should have an id")]
        public Guid Id { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Artist should have a stagename")]
        [StringLength(225, ErrorMessage = "Artist stagename should be less than 225 characters")]
        public string StageName { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Artist should have a name")]
        [StringLength(maximumLength: 225, ErrorMessage = "Artist name should be less than 225 characters")]
        public string Name { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage ="Artist should have an age")]
        public int Age { get; set; }
    }
}
