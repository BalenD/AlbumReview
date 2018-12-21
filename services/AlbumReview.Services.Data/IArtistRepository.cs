using AlbumReview.Data.Models;
using AlbumReview.Data.Repositories;
using AlbumReview.Services.Data.helpers;
using System;
using System.Threading.Tasks;

namespace AlbumReview.Services.Data
{
    public interface IArtistRepository : IRepositoryBase<Artist>
    {
        Task<PagedList<Artist>> GetArtistsAsync(string orderBy, string searchQuery, int pageNumber, int pageSize);
        Task<Artist> GetArtistAsync(Guid artistId);
    }
}
