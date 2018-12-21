using AlbumReview.Data;
using AlbumReview.Data.Models;
using AlbumReview.Data.Repositories;
using AlbumReview.Services.Data.DtoModels;
using AlbumReview.Services.Data.helpers;
using AlbumReview.Services.Web;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AlbumReview.Services.Data
{
    public class ReviewRepository : RepositoryBase<Review>, IReviewRepository
    {
        private IPropertyMappingService _propertyMappingService;

        public ReviewRepository(AlbumReviewContext context, IPropertyMappingService propertyMappingService) : base(context)
        {
            _propertyMappingService = propertyMappingService;
            _propertyMappingService.AddReviewPropertyMapping<ReviewDto, Review>();
        }

        public async Task<Review> GetReviewForAlbumAsync(Guid albumId, Guid reviewId)
        {
            return await _context.Reviews.Where(x => x.AlbumId == albumId && x.Id == reviewId).FirstOrDefaultAsync();
        }

        public async Task<PagedList<Review>> GetReviewsForAlbumAsync(Guid albumId, string orderBy, int pageNumber, int pageSize)
        {
            var collectionBeforePaging = await _context.Reviews.Where(x => x.AlbumId == albumId)
                           .ApplySort(orderBy, _propertyMappingService.GetPropertyMapping<ReviewDto, Review>()).ToListAsync();


            return PagedList<Review>.Create(collectionBeforePaging, pageNumber, pageSize);
        }
    }
}
