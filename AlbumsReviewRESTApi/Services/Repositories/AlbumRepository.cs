using AlbumsReviewRESTApi.context;
using AlbumsReviewRESTApi.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlbumsReviewRESTApi.Services.Repositories
{
    public class AlbumRepository : Repository , IAlbumRepository
    {

        public AlbumRepository(AlbumsReviewContext context) : base(context)
        {

        }



        public async Task<IEnumerable<Album>> GetAlbumsForArtistAsync(Guid artistId)
        {
            return await _context.Albums
                .Where(x => x.ArtistId == artistId)
                .OrderBy(x => x.Name)
                .ToListAsync();

        }


        public async Task AddAlbumForArtist(Guid artistId, Album album)
        {

            var artist = await _context.Artists.Where(x => x.Id == artistId).FirstOrDefaultAsync();

            if (artist != null)
            {
                if (album.Id == Guid.Empty)
                {
                    album.Id = Guid.NewGuid();
                }
                album.ArtistId = artist.Id;
                artist.Albums.Add(album);
            }


            //  fallback in case errors with above
            //if (album.Id == Guid.Empty)
            //{
            //    album.Id = Guid.NewGuid();
            //}

            //album.ArtistId = artistId;

            //await _context.Albums.AddAsync(album);

        }

        public void DeleteAlbum(Album album)
        {
            _context.Albums.Remove(album);
        }

        public async Task<Album> GetAlbumForArtistAsync(Guid albumId, Guid artistId)
        {
            return await _context.Albums
                .Where(x => x.ArtistId == artistId && x.Id == albumId)
                .FirstOrDefaultAsync();
        }

        public void UpdateAlbumForArtist(Guid artistId, Album album)
        {
            if (album.ArtistId == null)
            {
                album.ArtistId = artistId;
            }

            _context.Albums.Update(album);
        }
    }
}
