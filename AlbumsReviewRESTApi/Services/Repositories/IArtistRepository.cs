using AlbumsReviewRESTApi.Entities;
using AlbumsReviewRESTApi.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AlbumsReviewRESTApi.Services.Repositories
{
    public interface IArtistRepository : IRepository
    {
        Task<PagedList<Artist>> GetArtistsAsync(RequestParameters artistsRequestParameters);
        Task<Artist> GetArtistAsync(Guid artistId);
        void UpdateArtist(Artist artist);
        void DeleteArtist(Artist artist);
        void AddArtist(Artist artist);
    }
}
