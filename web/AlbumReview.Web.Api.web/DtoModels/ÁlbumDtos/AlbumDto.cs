using System;
using System.ComponentModel.DataAnnotations;

namespace AlbumReview.Web.Api.DtoModels
{
    public class AlbumDto
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Album should have an id")]
        public Guid Id { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Album should have a name")]
        [StringLength(225, ErrorMessage = "Album name should be less than 225 characters")]
        public string Name { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Album should have a release date")]
        public DateTime Released { get; set; }
        public int Age { get; set; }

    }
}
