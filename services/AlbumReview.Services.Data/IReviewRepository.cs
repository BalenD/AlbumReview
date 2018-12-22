using AlbumReview.Data.Models;
using System;
using System.Threading.Tasks;
using AlbumReview.Services.Data.helpers;
using AlbumReview.Data.Repositories;
using System.Collections.Generic;

namespace AlbumReview.Services.Data
{
    public interface IReviewRepository : IRepositoryBase<Review>
    {
        Task<Review> GetReviewForAlbumAsync(Guid albumId, Guid reviewId);
        Task<PagedList<Review>> GetReviewsForAlbumAsync(Guid albumId, string orderBy, int pageNumber, int pageSize, IDictionary<string, IEnumerable<string>> propertyMapping);
        //Task AddReviewForAlbumAsync(Guid albumId, Review reviewToCreate);
        //void UpdateReviewForAlbum(Guid albumID, Review reviewToUpdate);
        //void DeleteReviewForAlbum(Review reviewToDelete);
    }
}
