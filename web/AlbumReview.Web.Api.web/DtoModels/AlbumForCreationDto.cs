using System;
using System.ComponentModel.DataAnnotations;

namespace AlbumReview.Web.DtoModels
{
    public class AlbumForCreationDto
    {
        
        public Guid Id { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Album should have a name")]
        [StringLength(225, ErrorMessage = "Album name should be less than 225 characters")]
        public string Name { get; set; }
        [Required(AllowEmptyStrings = false , ErrorMessage = "Album should have a release date")]
        public DateTimeOffset Released { get; set; }
    }
}
