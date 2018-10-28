using AlbumsReviewRESTApi.context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace AlbumsReviewRESTApi.Services.Repositories
{
    public class Repository : IRepository
    {
        protected AlbumsReviewContext _context;

        public Repository(AlbumsReviewContext context)
        {
            _context = context;
        }
        public async Task<bool> ArtistExists(Guid artistId)
        {
            return await _context.Artists.AnyAsync(x => x.Id == artistId);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() > 0);
        }
    }
}
