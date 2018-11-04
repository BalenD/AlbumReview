using System;
using System.Linq;
using System.Threading.Tasks;
using AlbumsReviewRESTApi.context;
using AlbumsReviewRESTApi.Entities;
using AlbumsReviewRESTApi.Helpers;
using AlbumsReviewRESTApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AlbumsReviewRESTApi.Services.Repositories
{
    public class ArtistRepository : Repository, IArtistRepository
    {
        private IPropertyMappingService _propertyMappingService;

        public ArtistRepository(AlbumsReviewContext context, IPropertyMappingService propertyMappingService) : base( context)
        {
            _propertyMappingService = propertyMappingService;
        }

        public void AddArtist(Artist artist)
        {
            if (artist.Id == null)
            {
                artist.Id = Guid.NewGuid();
            }
            
            _context.Artists.Add(artist);
        }


        public void DeleteArtist(Artist artist)
        {
            _context.Artists.Remove(artist);
        }

        public async Task<Artist> GetArtistAsync(Guid artistId)
        {
            return await _context.Artists
                .FirstOrDefaultAsync(x => x.Id == artistId);
        }

        public async Task<PagedList<Artist>> GetArtistsAsync(RequestParameters artistsRequestParameters)
        {
            var collectionBeforePaging = await _context.Artists
                            .ApplySort(artistsRequestParameters.OrderBy, _propertyMappingService.GetPropertyMapping<ArtistDto, Artist>()).ToListAsync();
            

            if (!string.IsNullOrEmpty(artistsRequestParameters.SearchQuery))
            {
                var searchQueryForWhereClause = artistsRequestParameters.SearchQuery.Trim().ToLowerInvariant();

                collectionBeforePaging = collectionBeforePaging
                    .Where(a => a.StageName.ToLowerInvariant().Contains(searchQueryForWhereClause)
                    || a.FirstName.ToLowerInvariant().Contains(searchQueryForWhereClause)
                    || a.LastName.ToLowerInvariant().Contains(searchQueryForWhereClause)).ToList();
            }


            return PagedList<Artist>.Create(collectionBeforePaging.AsQueryable(), artistsRequestParameters.PageNumber, artistsRequestParameters.PageSize);

        }

        public void UpdateArtist(Artist artist)
        {
            _context.Artists.Update(artist);
        }
    }
}
