using AlbumsReviewRESTApi.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AlbumsReviewRESTApi.Services.Repositories
{
    public interface IAlbumRepository : IRepository
    {
        Task<IEnumerable<Album>> GetAlbumsForArtistAsync(Guid artistId);
        Task<Album> GetAlbumForArtistAsync(Guid albumId, Guid artistId);
        void AddAlbumForArtist(Guid artistId, Album album);
        void DeleteAlbum(Album album);
        void updateAlbumForArtist(Guid artistId, Album album);
        
    }
}
