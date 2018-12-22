using AlbumReview.Data;
using AlbumReview.Data.Models;
using AlbumReview.Data.Repositories;
using AlbumReview.Services.Data.helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlbumReview.Services.Data
{
    public class ReviewRepository : RepositoryBase<Review>, IReviewRepository
    {

        public ReviewRepository(AlbumReviewContext context) : base(context)
        {
        }

        public async Task<Review> GetReviewForAlbumAsync(Guid albumId, Guid reviewId)
        {
            return await _context.Reviews.Where(x => x.AlbumId == albumId && x.Id == reviewId).FirstOrDefaultAsync();
        }

        public async Task<PagedList<Review>> GetReviewsForAlbumAsync(
            Guid albumId,
            string orderBy,
            int pageNumber,
            int pageSize,
            IDictionary<string, IEnumerable<string>> propertyMapping)
        {
            var collectionBeforePaging = await _context.Reviews.Where(x => x.AlbumId == albumId)
                           .ApplySort(orderBy, propertyMapping).ToListAsync();


            return PagedList<Review>.Create(collectionBeforePaging, pageNumber, pageSize);
        }
    }
}
