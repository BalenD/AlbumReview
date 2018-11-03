using System;
using System.Threading.Tasks;

namespace AlbumsReviewRESTApi.Services.Repositories
{
    public interface IRepository
    {
        bool ArtistExists(Guid artistId);
        bool AlbumExists(Guid albumID);
        Task<bool> ArtistExistsAsync(Guid artistId);
        Task<bool> AlbumExistsAsync(Guid albumId);
        Task<bool> SaveChangesAsync();
    }
}
