using AlbumsReviewRESTApi.Entities;
using AlbumsReviewRESTApi.filters;
using AlbumsReviewRESTApi.Helpers;
using AlbumsReviewRESTApi.Models;
using AlbumsReviewRESTApi.Services;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
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
        public async Task<IActionResult> CreateAlbum([FromRoute] Guid artistId, [FromBody] AlbumForCreationDto albumToCreate)
        {
            if (artistId == Guid.Empty)
            {
                return BadRequest();
            }

            if (albumToCreate == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return new ErrorProcessingEntityObjectResult(ModelState);
            }

            var albumEntity = Mapper.Map<Album>(albumToCreate);

            _albumsReviewRepository.AddAlbumForArtist(artistId, albumEntity);

            if (!await _albumsReviewRepository.SaveChangesAsync())
            {
                throw new Exception($"adding album to artist {artistId} failed");
            }


            return CreatedAtRoute("GetAlbum", new { artistId, id = albumToCreate.Id }, albumEntity);

        }


        [HttpGet("{id}", Name = "GetAlbum")]
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

            if (!await _albumsReviewRepository.ArtistExists(artistId))
            {
                return NotFound();
            }

            var foundAlbum = await _albumsReviewRepository.GetAlbumForArtistAsync(id, artistId);

            if (foundAlbum == null)
            {
                return BadRequest();
            }

            return Ok(foundAlbum);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAlbum([FromRoute] Guid artistId, [FromRoute] Guid id, [FromBody] AlbumForUpdateDto albumToUpdate)
        {
            if (albumToUpdate == null)
            {
                return BadRequest();
            }

            if (artistId == Guid.Empty)
            {
                return BadRequest();
            }

            if (!await _albumsReviewRepository.ArtistExists(artistId))
            {
                return NotFound();
            }

            var albumFromRepo = await _albumsReviewRepository.GetAlbumForArtistAsync(id, artistId);

            //  Upserting
            if (albumFromRepo == null)
            {

                var albumEntity = Mapper.Map<Album>(albumToUpdate);
                albumEntity.Id = id;

                _albumsReviewRepository.AddAlbumForArtist(artistId, albumEntity);

                if (!await _albumsReviewRepository.SaveChangesAsync())
                {
                    throw new Exception($"Upserting album {albumEntity.Id} for artist {artistId} failed");
                }

                return CreatedAtRoute("GetAlbum", new { artistId, id = albumEntity.Id }, albumToUpdate);
            }


            Mapper.Map(albumToUpdate, albumFromRepo);

            _albumsReviewRepository.updateAlbumForArtist(artistId, albumFromRepo);

            if (!await _albumsReviewRepository.SaveChangesAsync())
            {
                throw new Exception($"Updating album {id} for author {artistId} failed on save");
            }

            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PartiallyUpdateAlbum([FromRoute] Guid artistId, [FromRoute] Guid id, JsonPatchDocument<Album> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            var albumFromRepo = await _albumsReviewRepository.GetAlbumForArtistAsync(id, artistId);

            //  Upserting

            if (albumFromRepo == null)
            {
                var album = new Album();

                patchDoc.ApplyTo(album, ModelState);

                TryValidateModel(album);

                if (!ModelState.IsValid)
                {
                    return new ErrorProcessingEntityObjectResult(ModelState);
                }

                album.Id = id;

                _albumsReviewRepository.AddAlbumForArtist(artistId, album);

                if (!await _albumsReviewRepository.SaveChangesAsync())
                {
                    throw new Exception($"Upserting album {album.Id} failed for artist {artistId}");
                }

                return CreatedAtRoute("GetAlbum", new { artistId, id = album.Id }, album);
            }


            patchDoc.ApplyTo(albumFromRepo, ModelState);

            TryValidateModel(albumFromRepo);

            if (!ModelState.IsValid)
            {
                return new ErrorProcessingEntityObjectResult(ModelState);
            }

            if (!await _albumsReviewRepository.SaveChangesAsync())
            {
                throw new Exception($"updating album {id} for artist {artistId} failed on save");
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAlbum([FromRoute] Guid artistId, [FromRoute] Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest();
            }

            if (artistId == Guid.Empty)
            {
                return BadRequest();
            }

            var albumFromRepo = await _albumsReviewRepository.GetAlbumForArtistAsync(id, artistId);

            if (albumFromRepo == null)
            {
                return NotFound();
            }

            _albumsReviewRepository.DeleteAlbum(albumFromRepo);

            if (!await _albumsReviewRepository.SaveChangesAsync())
            {
                throw new Exception($"deleting for album {id} failed");
            }

            return Ok();
        }
    }
}
