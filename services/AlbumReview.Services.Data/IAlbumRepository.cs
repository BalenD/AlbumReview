using AlbumReview.Data.Models;
using AlbumReview.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AlbumReview.Services.Data
{
    public interface IAlbumRepository : IRepositoryBase<Album>
    {
        Task<IEnumerable<Album>> GetAlbumsForArtistAsync(Guid artistId);
        Task<Album> GetAlbumForArtistAsync(Guid albumId, Guid artistId);
    }
}
