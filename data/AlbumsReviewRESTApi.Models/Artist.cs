using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlbumsReviewRESTApi.Data.Models
{
    [Table("Artists")]
    public class Artist
    {
        public Artist()
        {
            Albums = new List<Album>();
        }
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string StageName { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }
        [Required]
        public DateTimeOffset DateOfBirth { get; set; }
        public ICollection<Album> Albums { get; set; }
    }
}
