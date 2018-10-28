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


        public async void AddAlbumForArtist(Guid artistId, Album album)
        {

            //  TODO: find out why this doesnt work
            //var artist = await GetArtistAsync(artistId);

            //if (artist != null)
            //{
            //    if (album.Id == Guid.Empty)
            //    {
            //        album.Id = Guid.NewGuid();
            //    }
            //    album.ArtistId = artist.Id;
            //    artist.Albums.Add(album);
            //}

            if (album.Id == Guid.Empty)
            {
                album.Id = Guid.NewGuid();
            }

            album.ArtistId = artistId;

            await _context.Albums.AddAsync(album);

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

        public void updateAlbumForArtist(Guid artistId, Album album)
        {
            if (album.ArtistId == null)
            {
                album.ArtistId = artistId;
            }

            _context.Albums.Update(album);
        }
    }
}
