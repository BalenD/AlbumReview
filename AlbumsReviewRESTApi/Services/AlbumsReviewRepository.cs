using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AlbumsReviewRESTApi.context;
using AlbumsReviewRESTApi.Entities;
using AlbumsReviewRESTApi.Helpers;
using Microsoft.EntityFrameworkCore;

namespace AlbumsReviewRESTApi.Services
{
    public class AlbumsReviewRepository : IAlbumsReviewRepository
    {
        private AlbumsReviewContext _context;

        public AlbumsReviewRepository(AlbumsReviewContext context)
        {
            _context = context;
        }

        public async void AddAlbumForArtist(Guid artistId, Album album)
        {
            var artist = await GetArtistAsync(artistId);

            if (artist != null)
            {
                if (album.Id == Guid.Empty)
                {
                    album.Id = Guid.NewGuid();
                }

                artist.Albums.Add(album);
            }

        }

        public async void AddArtist(Artist artist)
        {
            if (artist.Id == null)
            {
                artist.Id = Guid.NewGuid();
            }
            
            await _context.Artists.AddAsync(artist);
        }

        public void DeleteArtistAsync(Artist artist)
        {
            _context.Artists.Remove(artist);
        }

        public async Task<Album> GetAlbumForArtistAsync(Guid albumId, Guid artistId)
        {
            return await _context.Albums
                .Where(x => x.ArtistId == artistId && x.Id == albumId)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Album>> GetAlbumsForArtistAsync(Guid artistId)
        {
            return await _context.Albums
                .Where(x => x.ArtistId == artistId)
                .OrderBy(x => x.Name)
                .ToListAsync();

        }

        public async Task<Artist> GetArtistAsync(Guid artistId)
        {
            return await _context.Artists.FirstOrDefaultAsync(x => x.Id == artistId);
        }

        public async Task<IEnumerable<Artist>> GetArtistsAsync(ArtistsRequestParameters artistsRequestParameters)
        {

            return await _context.Artists
                .OrderBy(x => x.StageName)
                .ToListAsync();

        }

        public async Task<IEnumerable<Review>> GetReviewsAsync(Guid albumId)
        {
            return await _context.Reviews
                .Where(x => x.AlbumId == albumId)
                .ToListAsync();
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() > 0);
        }

        public void UpdateArtistAsync(Artist artist)
        {
            if (artist.Id == null)
            {
                artist.Id = Guid.NewGuid();
            }
            _context.Artists.Update(artist);
        }
    }
}
