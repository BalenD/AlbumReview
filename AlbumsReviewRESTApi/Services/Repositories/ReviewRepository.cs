using AlbumsReviewRESTApi.context;
using AlbumsReviewRESTApi.Entities;
using AlbumsReviewRESTApi.Helpers;
using AlbumsReviewRESTApi.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AlbumsReviewRESTApi.Services.Repositories
{
    public class ReviewRepository : Repository, IReviewRepository
    {
        private IPropertyMappingService _propertyMappingService;

        public ReviewRepository(AlbumsReviewContext context, IPropertyMappingService propertyMappingService) : base(context)
        {
            _propertyMappingService = propertyMappingService;
        }

        public async Task AddReviewForAlbumAsync(Guid albumId, Review reviewToCreate)
        {
            var album = await _context.Albums.Where(x => x.Id == albumId).FirstOrDefaultAsync();

            if (album != null)
            {
                if (reviewToCreate.Id == Guid.Empty)
                {
                    reviewToCreate.Id = Guid.NewGuid();
                }
                reviewToCreate.AlbumId = albumId;
                album.Reviews.Add(reviewToCreate);
            }
        }

        public void DeleteReviewForAlbum(Review reviewToDelete)
        {
            _context.Reviews.Remove(reviewToDelete);
        }

        public async Task<Review> GetReviewForAlbumAsync(Guid albumId, Guid reviewId)
        {
            return await _context.Reviews.Where(x => x.AlbumId == albumId && x.Id == reviewId).FirstOrDefaultAsync();
        }

        public async Task<PagedList<Review>> GetReviewsForAlbumAsync(Guid albumId, RequestParameters reviewRequestParameters)
        {
            var collectionBeforePaging = await _context.Reviews.Where(x => x.AlbumId == albumId)
                           .ApplySort(reviewRequestParameters.OrderBy, _propertyMappingService.GetPropertyMapping<ReviewDto, Review>()).ToListAsync();


            //  possibly add some searching
            


            return PagedList<Review>.Create(collectionBeforePaging.AsQueryable(), reviewRequestParameters.PageNumber, reviewRequestParameters.PageSize);
        }

        public void UpdateReviewForAlbum(Guid albumID, Review reviewToUpdate)
        {
            if (reviewToUpdate.AlbumId == null)
            {
                reviewToUpdate.AlbumId = albumID;
            }
            _context.Reviews.Update(reviewToUpdate);
        }
    }
}
