using System;

namespace AlbumReview.Web.Api.DtoModels
{
    public class LinkedArtistDto
    {
        public Guid Id { get; set; }
        public string StageName { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }

    }
}
