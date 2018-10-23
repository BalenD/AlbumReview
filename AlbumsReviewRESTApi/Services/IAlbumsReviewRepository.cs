using AlbumsReviewRESTApi.Entities;
using AlbumsReviewRESTApi.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlbumsReviewRESTApi.Services
{
    public interface IAlbumsReviewRepository
    {
        Task<IEnumerable<Artist>> GetArtistsAsync(ArtistsRequestParameters artistsRequestParameters);
        Task<Artist> GetArtistAsync(Guid artistId);
        void UpdateArtistAsync(Artist artist);
        void DeleteArtistAsync(Artist artist);
        void AddArtist(Artist artist);
        Task<IEnumerable<Album>> GetAlbumsForArtistAsync(Guid artistId);
        Task<Album> GetAlbumForArtistAsync(Guid albumId, Guid artistId);

        void AddAlbumForArtist(Guid artistId, Album album);
        Task<IEnumerable<Review>> GetReviewsAsync(Guid albumID);
        Task<bool> SaveChangesAsync();

    }
}
