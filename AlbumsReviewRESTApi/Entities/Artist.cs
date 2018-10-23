using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlbumsReviewRESTApi.Entities
{
    [Table("Artists")]
    public class Artist
    {
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

        public DateTimeOffset DateOfBirth { get; set; }
        public ICollection<Album> Albums { get; set; }
    }
}
