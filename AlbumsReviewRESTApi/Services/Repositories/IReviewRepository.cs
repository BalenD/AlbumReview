using AlbumsReviewRESTApi.Entities;
using AlbumsReviewRESTApi.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AlbumsReviewRESTApi.Services.Repositories
{
    public interface IReviewRepository : IRepository
    {
        Task<Review> GetReviewForAlbumAsync(Guid albumId, Guid reviewId);
        Task<PagedList<Review>> GetReviewsForAlbumAsync(Guid albumId, RequestParameters reviewRequestParameters);
        Task AddReviewForAlbumAsync(Guid albumId, Review reviewToCreate);
        void UpdateReviewForAlbum(Guid albumID, Review reviewToUpdate);
        void DeleteReviewForAlbum(Review reviewToDelete);
    }
}
