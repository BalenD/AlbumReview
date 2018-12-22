using AlbumReview.Data;
using AlbumReview.Data.Models;
using AlbumReview.Services.Data.helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using AlbumReview.Data.Repositories;
using System.Collections.Generic;

namespace AlbumReview.Services.Data
{
    public class ArtistRepository : RepositoryBase<Artist>, IArtistRepository
    {

        public ArtistRepository(AlbumReviewContext context) : base(context)
        {
            
        }


        public async Task<Artist> GetArtistAsync(Guid artistId)
        {
            return await _context.Artists
                .FirstOrDefaultAsync(x => x.Id == artistId);
        }

        public async Task<PagedList<Artist>> GetArtistsAsync(
            string orderBy,
            string searchQuery,
            int pageNumber,
            int pageSize,
            IDictionary<string, IEnumerable<string>> propertyMapping)
        {
            var collectionBeforePaging = _context.Artists
                            .ApplySort(orderBy, propertyMapping);


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
