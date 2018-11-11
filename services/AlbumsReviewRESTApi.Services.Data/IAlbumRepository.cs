using AlbumsReviewRESTApi.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AlbumsReviewRESTApi.Services.Data
{
    public interface IAlbumRepository : IRepository
    {
        Task<IEnumerable<Album>> GetAlbumsForArtistAsync(Guid artistId);
        Task<Album> GetAlbumForArtistAsync(Guid albumId, Guid artistId);
        Task AddAlbumForArtist(Guid artistId, Album album);
        void DeleteAlbum(Album album);
        void UpdateAlbumForArtist(Guid artistId, Album album);
    }
}
