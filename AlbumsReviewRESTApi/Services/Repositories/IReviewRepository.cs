using AlbumsReviewRESTApi.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AlbumsReviewRESTApi.Services.Repositories
{
    public interface IReviewRepository : IRepository
    {
        Task<IEnumerable<Review>> GetReviewsForAlbumAsync(Guid albumID);
        Task<Review> GetReviewForAlbum(Guid albumId, Guid reviewId);
    }
}
