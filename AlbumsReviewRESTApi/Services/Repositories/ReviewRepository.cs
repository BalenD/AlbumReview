using AlbumsReviewRESTApi.context;
using AlbumsReviewRESTApi.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlbumsReviewRESTApi.Services.Repositories
{
    public class ReviewRepository : Repository, IReviewRepository
    {
        public ReviewRepository(AlbumsReviewContext context) : base(context)
        {

        }

        public Task<Review> GetReviewForAlbum(Guid albumId, Guid reviewId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Review>> GetReviewsForAlbumAsync(Guid albumId)
        {
            return await _context.Reviews
                .Where(x => x.AlbumId == albumId)
                .ToListAsync();
        }

       
    }
}
