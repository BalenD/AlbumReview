using System;
using System.ComponentModel.DataAnnotations;

namespace AlbumReview.Web.Api.DtoModels
{
    public class AlbumForUpdateDto
    {
        [StringLength(maximumLength: 225, ErrorMessage = "Album name should be less than 100 characters")]
        public string Name { get; set; }
        public DateTimeOffset Released { get; set; }
    }
}
