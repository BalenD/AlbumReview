using System;
using System.Threading.Tasks;

namespace AlbumsReviewRESTApi.Services.Repositories
{
    public interface IRepository
    {
        Task<bool> ArtistExists(Guid artistId);
        Task<bool> SaveChangesAsync();
    }
}
