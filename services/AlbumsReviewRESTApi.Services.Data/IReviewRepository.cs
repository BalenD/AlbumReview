using AlbumsReviewRESTApi.Data.Models;
using System;
using System.Threading.Tasks;
using AlbumsReviewRESTApi.Services.Data.helpers;
namespace AlbumsReviewRESTApi.Services.Data
{
    public interface IReviewRepository : IRepository
    {
        Task<Review> GetReviewForAlbumAsync(Guid albumId, Guid reviewId);
        Task<PagedList<Review>> GetReviewsForAlbumAsync(Guid albumId, string orderBy, int pageNumber, int pageSize);
        Task AddReviewForAlbumAsync(Guid albumId, Review reviewToCreate);
        void UpdateReviewForAlbum(Guid albumID, Review reviewToUpdate);
        void DeleteReviewForAlbum(Review reviewToDelete);
    }
}
