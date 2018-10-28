using AlbumsReviewRESTApi.Entities;
using AlbumsReviewRESTApi.filters;
using AlbumsReviewRESTApi.Helpers;
using AlbumsReviewRESTApi.Models;
using AlbumsReviewRESTApi.Services;
using AlbumsReviewRESTApi.Services.Repositories;
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
        private IAlbumRepository _albumsReviewRepository;
        private IPropertyMappingService _propertyMappingService;

        public AlbumsController(IAlbumRepository albumsReviewRepository, IPropertyMappingService propertyMappingService)
        {
            _albumsReviewRepository = albumsReviewRepository;
            _propertyMappingService = propertyMappingService;
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
            if (id == Guid.Empty)
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
        public async Task<IActionResult> PartiallyUpdateAlbum([FromRoute] Guid artistId, [FromRoute] Guid id, JsonPatchDocument<AlbumForUpdateDto> patchDoc)
        {
            if (patchDoc == null)
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
                var albumForUpdateDto = new AlbumForUpdateDto();

                patchDoc.ApplyTo(albumForUpdateDto, ModelState);

                TryValidateModel(albumForUpdateDto);

                if (!ModelState.IsValid)
                {
                    return new ErrorProcessingEntityObjectResult(ModelState);
                }

                var albumToAdd = Mapper.Map<Album>(albumForUpdateDto);

                albumToAdd.Id = id;

                _albumsReviewRepository.AddAlbumForArtist(artistId, albumToAdd);

                if (!await _albumsReviewRepository.SaveChangesAsync())
                {
                    throw new Exception($"Upserting album {albumToAdd.Id} failed for artist {artistId}");
                }

                var albumToReturn = Mapper.Map<AlbumDto>(albumToAdd);

                return CreatedAtRoute("GetAlbum", new { artistId, id = albumToReturn.Id }, albumToReturn);
            }

            var albumToPatch = Mapper.Map<AlbumForUpdateDto>(albumFromRepo);

            patchDoc.ApplyTo(albumToPatch, ModelState);

            TryValidateModel(albumToPatch);

            if (!ModelState.IsValid)
            {
                return new ErrorProcessingEntityObjectResult(ModelState);
            }


            Mapper.Map(albumToPatch, albumFromRepo);

            _albumsReviewRepository.updateAlbumForArtist(artistId, albumFromRepo);

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
