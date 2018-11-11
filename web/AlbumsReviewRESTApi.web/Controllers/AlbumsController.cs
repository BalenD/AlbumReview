using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using AlbumsReviewRESTApi.Web.filters;
using AlbumsReviewRESTApi.Services.Web;
using AlbumsReviewRESTApi.Services.Data;
using AlbumsReviewRESTApi.Data.Models;
using AlbumsReviewRESTApi.Services.Data.DtoModels;
using AlbumsReviewRESTApi.Web.Helpers;

namespace AlbumsReviewRESTApi.Web.Controllers
{

    [Route("api/artists/{artistId}/albums")]
    [RouteParameterValidationFilter]
    [TypeFilter(typeof(RouteParameterResourceValidationFilterAttribute))]
    public class AlbumsController : Controller
    {
        private IAlbumRepository _albumRepository;

        public AlbumsController(IAlbumRepository albumsReviewRepository, IPropertyMappingService propertyMappingService)
        {
            _albumRepository = albumsReviewRepository;
        }

        [HttpGet]
        [AlbumResultFilter]
        public async Task<IActionResult> GetAlbums([FromRoute] Guid artistId)
        {
            var albumEntities = await _albumRepository.GetAlbumsForArtistAsync(artistId);
            return Ok(albumEntities);
        }

        [HttpPost]
        [AlbumResultFilter]
        public async Task<IActionResult> CreateAlbum([FromRoute] Guid artistId, [FromBody] AlbumForCreationDto albumToCreate)
        {

            if (albumToCreate == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return new ErrorProcessingEntityObjectResult(ModelState);
            }

            var albumEntity = Mapper.Map<Album>(albumToCreate);

            albumEntity.Id = Guid.NewGuid();

            await _albumRepository.AddAlbumForArtist(artistId, albumEntity);

            if (!await _albumRepository.SaveChangesAsync())
            {
                throw new Exception($"adding album to artist {artistId} failed");
            }


            return CreatedAtRoute("GetAlbum", new { artistId, id = albumEntity.Id }, albumEntity);

        }


        [HttpGet("{albumId}", Name = "GetAlbum")]
        [AlbumResultFilter]
        public async Task<IActionResult> GetAlbum([FromRoute] Guid albumId, [FromRoute] Guid artistId)
        {
            var foundAlbum = await _albumRepository.GetAlbumForArtistAsync(albumId, artistId);

            if (foundAlbum == null)
            {
                return BadRequest();
            }

            return Ok(foundAlbum);
        }

        [HttpPut("{albumId}")]
        public async Task<IActionResult> UpdateAlbum([FromRoute] Guid artistId, [FromRoute] Guid albumId, [FromBody] AlbumForUpdateDto albumToUpdate)
        {
            if (albumToUpdate == null)
            {
                return BadRequest();
            }

            var albumFromRepo = await _albumRepository.GetAlbumForArtistAsync(albumId, artistId);

            //  Upserting
            if (albumFromRepo == null)
            {

                var albumEntity = Mapper.Map<Album>(albumToUpdate);
                albumEntity.Id = albumId;

                await _albumRepository.AddAlbumForArtist(artistId, albumEntity);

                if (!await _albumRepository.SaveChangesAsync())
                {
                    throw new Exception($"Upserting album {albumEntity.Id} for artist {artistId} failed");
                }

                return CreatedAtRoute("GetAlbum", new { artistId, albumId = albumEntity.Id }, albumToUpdate);
            }


            Mapper.Map(albumToUpdate, albumFromRepo);

            _albumRepository.UpdateAlbumForArtist(artistId, albumFromRepo);

            if (!await _albumRepository.SaveChangesAsync())
            {
                throw new Exception($"Updating album {albumId} for author {artistId} failed on save");
            }

            return NoContent();
        }

        [HttpPatch("{albumId}")]
        public async Task<IActionResult> PartiallyUpdateAlbum([FromRoute] Guid artistId, [FromRoute] Guid albumId, JsonPatchDocument<AlbumForUpdateDto> patchDoc)
        {

            if (patchDoc == null)
            {
                return BadRequest();
            }

            var albumFromRepo = await _albumRepository.GetAlbumForArtistAsync(albumId, artistId);

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

                albumToAdd.Id = albumId;

                await _albumRepository.AddAlbumForArtist(artistId, albumToAdd);

                if (!await _albumRepository.SaveChangesAsync())
                {
                    throw new Exception($"Upserting album {albumToAdd.Id} failed for artist {artistId}");
                }

                var albumToReturn = Mapper.Map<AlbumDto>(albumToAdd);

                return CreatedAtRoute("GetAlbum", new { artistId, albumId = albumToReturn.Id }, albumToReturn);
            }

            var albumToPatch = Mapper.Map<AlbumForUpdateDto>(albumFromRepo);

            patchDoc.ApplyTo(albumToPatch, ModelState);

            TryValidateModel(albumToPatch);

            if (!ModelState.IsValid)
            {
                return new ErrorProcessingEntityObjectResult(ModelState);
            }


            Mapper.Map(albumToPatch, albumFromRepo);

            _albumRepository.UpdateAlbumForArtist(artistId, albumFromRepo);

            if (!await _albumRepository.SaveChangesAsync())
            {
                throw new Exception($"updating album {albumId} for artist {artistId} failed on save");
            }

            return NoContent();
        }

        [HttpDelete("{albumId}")]
        public async Task<IActionResult> DeleteAlbum([FromRoute] Guid artistId, [FromRoute] Guid albumId)
        {


            var albumFromRepo = await _albumRepository.GetAlbumForArtistAsync(albumId, artistId);

            if (albumFromRepo == null)
            {
                return NotFound();
            }

            _albumRepository.DeleteAlbum(albumFromRepo);

            if (!await _albumRepository.SaveChangesAsync())
            {
                throw new Exception($"deleting for album {albumId} failed");
            }

            return Ok();
        }
    }
}
