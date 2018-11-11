using AlbumsReviewRESTApi.Data;
using AlbumsReviewRESTApi.Data.Models;
using AlbumsReviewRESTApi.Services.Data.DtoModels;
using AlbumsReviewRESTApi.Services.Data.helpers;
using AlbumsReviewRESTApi.Services.Web;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AlbumsReviewRESTApi.Services.Data
{
    public class ReviewRepository : Repository, IReviewRepository
    {
        private IPropertyMappingService _propertyMappingService;

        public ReviewRepository(AlbumsReviewRESTApiContext context, IPropertyMappingService propertyMappingService) : base(context)
        {
            _propertyMappingService = propertyMappingService;
            _propertyMappingService.AddReviewPropertyMapping<ReviewDto, Review>();
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

        public async Task<PagedList<Review>> GetReviewsForAlbumAsync(Guid albumId, string orderBy, int pageNumber, int pageSize)
        {
            var collectionBeforePaging = await _context.Reviews.Where(x => x.AlbumId == albumId)
                           .ApplySort(orderBy, _propertyMappingService.GetPropertyMapping<ReviewDto, Review>()).ToListAsync();


            //  possibly add some searching



            return PagedList<Review>.Create(collectionBeforePaging.AsQueryable(), pageNumber, pageSize);
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
