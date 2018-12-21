using AlbumReview.Data;
using AlbumReview.Data.Models;
using AlbumReview.Services.Data.helpers;
using AlbumReview.Services.Web;
using AlbumReview.Services.Data.DtoModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using AlbumReview.Data.Repositories;

namespace AlbumReview.Services.Data
{
    public class ArtistRepository : RepositoryBase<Artist>, IArtistRepository
    {
        private IPropertyMappingService _propertyMappingService;

        public ArtistRepository(AlbumReviewContext context, IPropertyMappingService propertyMappingService) : base(context)
        {
            _propertyMappingService = propertyMappingService;
            _propertyMappingService.AddArtistPropertyMapping<ArtistDto, Artist>();
            
        }


        public async Task<Artist> GetArtistAsync(Guid artistId)
        {
            return await _context.Artists
                .FirstOrDefaultAsync(x => x.Id == artistId);
        }

        public async Task<PagedList<Artist>> GetArtistsAsync(string orderBy, string searchQuery, int pageNumber, int pageSize)
        {
            var collectionBeforePaging = _context.Artists
                            .ApplySort(orderBy, _propertyMappingService.GetPropertyMapping<ArtistDto, Artist>());


            if (!string.IsNullOrEmpty(searchQuery))
            {
                var searchQueryForWhereClause = searchQuery.Trim().ToLowerInvariant();

                collectionBeforePaging = collectionBeforePaging
                    .Where(a => a.StageName.ToLowerInvariant().Contains(searchQueryForWhereClause)
                    || a.FirstName.ToLowerInvariant().Contains(searchQueryForWhereClause)
                    || a.LastName.ToLowerInvariant().Contains(searchQueryForWhereClause));
            }


            return PagedList<Artist>.Create(await collectionBeforePaging.ToListAsync(), pageNumber, pageSize);

        }
    }
}
