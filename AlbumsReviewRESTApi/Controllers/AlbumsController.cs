using AlbumsReviewRESTApi.Entities;
using AlbumsReviewRESTApi.filters;
using AlbumsReviewRESTApi.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlbumsReviewRESTApi.Controllers
{
    [Route("api/artists/{artistId}/albums")]
    public class AlbumsController : Controller
    {
        private IAlbumsReviewRepository _albumsReviewRepository;

        public AlbumsController(IAlbumsReviewRepository albumsReviewRepository)
        {
            _albumsReviewRepository = albumsReviewRepository;
        }

        [HttpGet]
        [AlbumResultFilter]
        public async Task<IActionResult> GetAlbums([FromRoute] Guid artistId)
        {
            var albumEntities = await _albumsReviewRepository.GetAlbumsForArtistAsync(artistId);
            return Ok(albumEntities);
        }

        [HttpPost]
        [AlbumResultFilter]
        public async Task<IActionResult> CreateAlbum([FromRoute] Guid artistId, [FromBody] Album albumToCreate)
        {
            
        }


        [HttpGet("{id}")]
        [AlbumResultFilter]
        public async Task<IActionResult> GetAlbum([FromRoute] Guid id, [FromRoute] Guid artistId)
        {
            if (id == null)
            {
                return BadRequest();
            }

            if (artistId == null)
            {
                return BadRequest();
            }

            var foundAlbum = await _albumsReviewRepository.GetAlbumForArtistAsync(id, artistId);

            if (foundAlbum == null)
            {
                return BadRequest();
            }

            return Ok(foundAlbum);
        }
    }
}
