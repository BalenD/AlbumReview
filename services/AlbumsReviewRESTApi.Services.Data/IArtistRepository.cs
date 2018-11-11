using AlbumsReviewRESTApi.Data.Models;
using AlbumsReviewRESTApi.Services.Data.helpers;
using System;
using System.Threading.Tasks;

namespace AlbumsReviewRESTApi.Services.Data
{
    public interface IArtistRepository : IRepository
    {
        Task<PagedList<Artist>> GetArtistsAsync(string orderBy, string searchQuery, int pageNumber, int pageSize);
        Task<Artist> GetArtistAsync(Guid artistId);
        void UpdateArtist(Artist artist);
        void DeleteArtist(Artist artist);
        void AddArtist(Artist artist);
    }
}
