using AlbumsReviewRESTApi.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AlbumsReviewRESTApi.Services.Data
{
    public class Repository : IRepository
    {
        protected AlbumsReviewRESTApiContext _context;

        public Repository(AlbumsReviewRESTApiContext context)
        {
            _context = context;
        }

        public bool AlbumExists(Guid albumID)
        {
            return _context.Albums.Any(x => x.Id == albumID);
        }

        public async Task<bool> AlbumExistsAsync(Guid albumId)
        {
            return await _context.Albums.AnyAsync(x => x.Id == albumId);
        }

        public bool ArtistExists(Guid artistId)
        {
            return _context.Artists.Any(x => x.Id == artistId);
        }

        public async Task<bool> ArtistExistsAsync(Guid artistId)
        {
            return await _context.Artists.AnyAsync(x => x.Id == artistId);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() > 0);
        }
    }
}
