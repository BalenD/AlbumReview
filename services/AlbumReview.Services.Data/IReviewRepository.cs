using AlbumReview.Data.Models;
using System;
using System.Threading.Tasks;
using AlbumReview.Services.Data.helpers;
using AlbumReview.Data.Repositories;

namespace AlbumReview.Services.Data
{
    public interface IReviewRepository : IRepositoryBase<Review>
    {
        Task<Review> GetReviewForAlbumAsync(Guid albumId, Guid reviewId);
        Task<PagedList<Review>> GetReviewsForAlbumAsync(Guid albumId, string orderBy, int pageNumber, int pageSize);
        //Task AddReviewForAlbumAsync(Guid albumId, Review reviewToCreate);
        //void UpdateReviewForAlbum(Guid albumID, Review reviewToUpdate);
        //void DeleteReviewForAlbum(Review reviewToDelete);
    }
}
