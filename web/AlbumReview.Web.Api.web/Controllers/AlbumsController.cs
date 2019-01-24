using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using AlbumReview.Web.Api.filters;
using AlbumReview.Services.Web;
using AlbumReview.Services.Data;
using AlbumReview.Data.Models;
using AlbumReview.Web.Api.DtoModels;
using AlbumReview.Web.Api.Helpers;
using AlbumReview.Web.Api.Api.Helpers;
using AlbumReview.Web.Api.services;
using System.Collections.Generic;
using System.Linq;
using System.Dynamic;

namespace AlbumReview.Web.Api.Controllers
{

    [Route("api/artists/{artistId}/albums")]
    [ApiController]
    [RouteParameterValidationFilter]
    public class AlbumsController : Controller
    {
        private readonly IAlbumRepository _albumRepository;
        private readonly IControllerHelper _controllerHelper;
        private readonly IHateoasHelper _hateoasHelper;
        private readonly ITypeHelperService _typeHelperService;

        public AlbumsController(IAlbumRepository albumsReviewRepository, IPropertyMappingService propertyMappingService,
                                IControllerHelper controllerHelper, IHateoasHelper hateoasHelper, ITypeHelperService typeHelperService)
        {
            _albumRepository = albumsReviewRepository;
            _controllerHelper = controllerHelper;
            _hateoasHelper = hateoasHelper;
            _typeHelperService = typeHelperService;
        }

        [HttpGet]
        [AlbumResultFilter]
        public async Task<IActionResult> GetAlbums([FromRoute] Guid artistId, [FromQuery] string fields)
        {

            var albumEntities = await _albumRepository.GetAlbumsForArtistAsync(artistId);

            var albumsToReturn = Mapper.Map<AlbumDto>(albumEntities);

            if (!string.IsNullOrWhiteSpace(fields))
            {
                if (!_typeHelperService.TypeHasProperties<AlbumDto>(fields))
                {
                    return BadRequest();
                }

                var shapedAlbumsToReturn = albumsToReturn.ShapeData(fields);
                return Ok(shapedAlbumsToReturn);
            }
            else
            {
                return Ok(albumsToReturn);
            }
            
        }

        [RequestMatchesMediaTypeHeader("Content-Type", new string[] { "application/vnd.BD.json+hateoas" })]
        public async Task<IActionResult> GetAlbumsWithHateoas([FromRoute] Guid artistId, [FromQuery] string fields)
        {
            if (!_typeHelperService.TypeHasProperties<AlbumDto>(fields))
            {
                return BadRequest();
            }

            var albumEntities = await _albumRepository.GetAlbumsForArtistAsync(artistId);

            var albums = Mapper.Map<IEnumerable<AlbumDto>>(albumEntities);

            var shapedAlbums = albums.Select(album =>
            {
                return _controllerHelper.ShapeAndAddLinkToObject(album, "Album", fields);
            });

            var resourceLinks = _hateoasHelper.CreateLinksForChildResources("Album", new { artistId });

            var linkedResourceCollection = new ExpandoObject();
            ((IDictionary<string, object>)linkedResourceCollection).Add("records", shapedAlbums);
            ((IDictionary<string, object>)linkedResourceCollection).Add("links", resourceLinks);
            return Ok(linkedResourceCollection);

        }

        [HttpPost]
        [AlbumResultFilter]
        public async Task<IActionResult> CreateAlbum([FromRoute] Guid artistId, [FromBody] AlbumForCreationDto albumToCreate)
        {

            var albumEntity = Mapper.Map<Album>(albumToCreate);

            albumEntity.Id = Guid.NewGuid();
            albumEntity.ArtistId = artistId;

            _albumRepository.Create(albumEntity);

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


            var albumFromRepo = await _albumRepository.GetAlbumForArtistAsync(albumId, artistId);

            //  Upserting
            if (albumFromRepo == null)
            {

                var albumEntity = Mapper.Map<Album>(albumToUpdate);
                albumEntity.Id = albumId;
                albumEntity.ArtistId = artistId;

                _albumRepository.Create(albumEntity);

                if (!await _albumRepository.SaveChangesAsync())
                {
                    throw new Exception($"Upserting album {albumEntity.Id} for artist {artistId} failed");
                }

                return CreatedAtRoute("GetAlbum", new { artistId, albumId = albumEntity.Id }, albumToUpdate);
            }


            Mapper.Map(albumToUpdate, albumFromRepo);
            _albumRepository.Update(albumFromRepo);

            if (!await _albumRepository.SaveChangesAsync())
            {
                throw new Exception($"Updating album {albumId} for author {artistId} failed on save");
            }

            return NoContent();
        }

        [HttpPatch("{albumId}")]
        public async Task<IActionResult> PartiallyUpdateAlbum([FromRoute] Guid artistId, [FromRoute] Guid albumId, JsonPatchDocument<AlbumForUpdateDto> patchDoc)
        {

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
                albumToAdd.ArtistId = artistId;

                _albumRepository.Create(albumToAdd);

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

            _albumRepository.Update(albumFromRepo);

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

            _albumRepository.Delete(albumFromRepo);

            if (!await _albumRepository.SaveChangesAsync())
            {
                throw new Exception($"deleting for album {albumId} failed");
            }

            return Ok();
        }
    }
}
