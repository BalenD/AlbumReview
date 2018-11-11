using System;
using AlbumsReviewRESTApi.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace AlbumsReviewRESTApi.Data
{
    public class AlbumsReviewRESTApiContext : DbContext
    {
        public DbSet<Album> Albums { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public AlbumsReviewRESTApiContext(DbContextOptions<AlbumsReviewRESTApiContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Review>().HasData(

                new Review()
                {
                    Id = Guid.Parse("BC62A8EA-2C2E-4EC2-A2A0-B4C48B523873"),
                    AlbumId = Guid.Parse("BB5C8E98-548F-4766-B3FB-8C65A1B3A390"),
                    Rating = 5,
                    Submitted = new DateTimeOffset(new DateTime(2018, 10, 06)),
                    SubmittedReview = "A great Album"
                },
                new Review()
                {
                    Id = Guid.Parse("77534F47-9088-4207-97A1-E09D3D3B670A"),
                    AlbumId = Guid.Parse("BB5C8E98-548F-4766-B3FB-8C65A1B3A390"),
                    Rating = 10,
                    Submitted = new DateTimeOffset(new DateTime(2018, 9, 5)),
                    SubmittedReview = "The best i have listened to"
                },
                new Review()
                {
                    Id = Guid.Parse("05DE4A52-84EE-4BCE-9921-9F1FC7912A69"),
                    AlbumId = Guid.Parse("00D676D5-FDF8-4F26-816F-B3775C1B1E1A"),
                    Rating = 6,
                    Submitted = new DateTimeOffset(new DateTime(2018, 2, 6)),
                    SubmittedReview = "A Lovely piece"
                },
                new Review()
                {
                    Id = Guid.Parse("A1806419-0BA2-40B8-8528-919499A98BE7"),
                    AlbumId = Guid.Parse("00D676D5-FDF8-4F26-816F-B3775C1B1E1A"),
                    Rating = 4,
                    Submitted = new DateTimeOffset(new DateTime(2018, 8, 6)),
                    SubmittedReview = "LOVED IT!"
                },
                new Review()
                {
                    Id = Guid.Parse("FF6CD8E9-F886-4CC4-B9BF-0962609BFC47"),
                    AlbumId = Guid.Parse("4AB21A9F-2041-4847-BEFB-57711B73F8D1"),
                    Rating = 9,
                    Submitted = new DateTimeOffset(new DateTime(2018, 8, 25)),
                    SubmittedReview = "DANNY BROWN SPAT FIRE"
                },
                new Review()
                {
                    Id = Guid.Parse("{597BD736-AFA1-40A3-B72A-38A1DE525A2F}"),
                    AlbumId = Guid.Parse("4AB21A9F-2041-4847-BEFB-57711B73F8D1"),
                    Rating = 8,
                    Submitted = new DateTimeOffset(new DateTime(2018, 5, 25)),
                    SubmittedReview = "DANNY BROWN IS GOAT"
                }
                );
            modelBuilder.Entity<Album>().HasData(
                new Album()
                {
                    Id = Guid.Parse("BB5C8E98-548F-4766-B3FB-8C65A1B3A390"),
                    ArtistId = Guid.Parse("05638AD6-F6B9-40F3-BE72-BD220295D059"),
                    Name = "Honor Killed The Samurai",
                    Released = new DateTimeOffset(new DateTime(2016, 8, 13)),

                },
                new Album()
                {
                    Id = Guid.Parse("00D676D5-FDF8-4F26-816F-B3775C1B1E1A"),
                    ArtistId = Guid.Parse("2F3E0481-33FE-4283-B4B4-72E866ACF5E8"),
                    Name = "To Pimp a Butterfly",
                    Released = new DateTimeOffset(new DateTime(2015, 5, 15)),

                },
                new Album()
                {
                    Id = Guid.Parse("4AB21A9F-2041-4847-BEFB-57711B73F8D1"),
                    ArtistId = Guid.Parse("EFC6DF62-7C32-4916-8920-B510C3E11907"),
                    Name = "Atrocity Exhibition",
                    Released = new DateTimeOffset(new DateTime(2016, 9, 27)),

                }
                );

            modelBuilder.Entity<Artist>().HasData(
                new Artist()
                {
                    Id = Guid.Parse("05638AD6-F6B9-40F3-BE72-BD220295D059"),
                    StageName = "Ka",
                    FirstName = "Kaseem",
                    LastName = "Ryan",
                    DateOfBirth = new DateTimeOffset(new DateTime(1972, 8, 11))
                },
                new Artist()
                {
                    Id = Guid.Parse("2F3E0481-33FE-4283-B4B4-72E866ACF5E8"),
                    StageName = "Kendrick Lamar",
                    FirstName = "Kendrick",
                    LastName = "Lamar Duckworth",
                    DateOfBirth = new DateTimeOffset(new DateTime(1987, 6, 17))
                },
                new Artist()
                {
                    Id = Guid.Parse("EFC6DF62-7C32-4916-8920-B510C3E11907"),
                    StageName = "Danny Brown",
                    FirstName = "Daniel",
                    LastName = "Dewan Sewell",
                    DateOfBirth = new DateTimeOffset(new DateTime(1981, 3, 16))
                }

                );
            base.OnModelCreating(modelBuilder);
        }
    }
}
