using AlbumReview.Data;
using AlbumReview.Data.Models;
using AlbumReview.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlbumReview.Services.Data
{
    public class AlbumRepository : RepositoryBase<Album>, IAlbumRepository
    {
        public AlbumRepository(AlbumReviewContext context) : base(context)
        {

        }

        public async Task<IEnumerable<Album>> GetAlbumsForArtistAsync(Guid artistId)
        {
            return await _context.Albums
                .Where(x => x.ArtistId == artistId)
                .OrderBy(x => x.Name)
                .ToListAsync();

        }


        public async Task<Album> GetAlbumForArtistAsync(Guid albumId, Guid artistId)
        {
            return await _context.Albums
                .Where(x => x.ArtistId == artistId && x.Id == albumId)
                .FirstOrDefaultAsync();
        }
    }
}
