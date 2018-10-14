using AlbumsReviewRESTApi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlbumsReviewRESTApi.Services
{
    public interface IAlbumsReviewRepository
    {
        Task<IEnumerable<Artist>> GetArtistsAsync();
        Task<Artist> GetArtistAsync(Guid artistId);
        void UpdateArtistAsync(Artist artist);
        void DeleteArtistAsync(Artist artist);
        void AddArtist(Artist artist);
        Task<IEnumerable<Album>> GetAlbumsAsync(Guid artistId);
        Task<IEnumerable<Review>> GetReviewsAsync(Guid albumID);
        Task<bool> SaveChangesAsync();

    }
}
