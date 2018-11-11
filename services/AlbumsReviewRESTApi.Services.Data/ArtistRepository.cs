using AlbumsReviewRESTApi.Data;
using AlbumsReviewRESTApi.Data.Models;
using AlbumsReviewRESTApi.Services.Data.helpers;
using AlbumsReviewRESTApi.Services.Web;
using AlbumsReviewRESTApi.Services.Data.DtoModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AlbumsReviewRESTApi.Services.Data
{
    public class ArtistRepository : Repository, IArtistRepository
    {
        private IPropertyMappingService _propertyMappingService;

        public ArtistRepository(AlbumsReviewRESTApiContext context, IPropertyMappingService propertyMappingService) : base(context)
        {
            _propertyMappingService = propertyMappingService;
            _propertyMappingService.AddArtistPropertyMapping<ArtistDto, Artist>();
            
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

        public async Task<PagedList<Artist>> GetArtistsAsync(string orderBy, string searchQuery, int pageNumber, int pageSize)
        {
            var collectionBeforePaging = await _context.Artists
                            .ApplySort(orderBy, _propertyMappingService.GetPropertyMapping<ArtistDto, Artist>()).ToListAsync();


            if (!string.IsNullOrEmpty(searchQuery))
            {
                var searchQueryForWhereClause = searchQuery.Trim().ToLowerInvariant();

                collectionBeforePaging = collectionBeforePaging
                    .Where(a => a.StageName.ToLowerInvariant().Contains(searchQueryForWhereClause)
                    || a.FirstName.ToLowerInvariant().Contains(searchQueryForWhereClause)
                    || a.LastName.ToLowerInvariant().Contains(searchQueryForWhereClause)).ToList();
            }


            return PagedList<Artist>.Create(collectionBeforePaging.AsQueryable(), pageNumber, pageSize);

        }

        public void UpdateArtist(Artist artist)
        {
            _context.Artists.Update(artist);
        }
    }
}
