using AlbumReview.Web.Api.filters;
using Microsoft.AspNetCore.JsonPatch;
using AlbumReview.Web.Api.Helpers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using AutoMapper;
using System.Collections.Generic;
using System.Dynamic;
using AlbumReview.Services.Web;
using AlbumReview.Services.Data;
using AlbumReview.Data.Models;
using AlbumReview.Web.Api.DtoModels;
using Microsoft.Extensions.Logging;
using AlbumReview.Web.Api.services;
using Newtonsoft.Json;
using AlbumReview.Services.Data.helpers;
using AlbumReview.Web.Api.Api.Helpers;
using System.Linq;

namespace AlbumReview.Web.Api.Controllers
{

    [Route("api/artists")]
    [ApiController]
    public class ArtistsController : Controller
    {
        private readonly IArtistRepository _artistRepository;
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly IHateoasHelper _hateoasHelper;
        private readonly ITypeHelperService _typeHelperService;
        private readonly ILogger<ArtistsController> _logger;
        private readonly IControllerHelper _controllerHelper;

        public ArtistsController(
            IArtistRepository artistRepository, IPropertyMappingService propertyMappingService,
            ITypeHelperService typeHelperService, IUrlHelper urlHelper, ILogger<ArtistsController> logger,
            IPaginationUrlHelper paginationUrlHelper, IHateoasHelper hateoasHelper, IControllerHelper controllerHelper)
        {
            _artistRepository = artistRepository;
            _typeHelperService = typeHelperService;
            _logger = logger;
            _controllerHelper = controllerHelper;
            _propertyMappingService = propertyMappingService;
            _hateoasHelper = hateoasHelper;
        }

        [HttpGet(Name = "GetArtists")]
        public async Task<IActionResult> GetArtists([FromQuery] RequestParameters requestParameter)
        {
            if (string.IsNullOrWhiteSpace(requestParameter.OrderBy))
            {
                requestParameter.OrderBy = "StageName";
            }

            var artistPagedList = await GetPagedListOfArtists(requestParameter);

            var paginationMetaData = _controllerHelper.CreatePaginationMetadataObject(artistPagedList, requestParameter, "GetArtists");


            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paginationMetaData));

            var artists = Mapper.Map<IEnumerable<ArtistDto>>(artistPagedList);

            if (requestParameter.IncludeMetadata)
            {
                var artistsWithMetadata = new EntityWithPaginationMetadataDto<ArtistDto>(paginationMetaData, artists);
                return Ok(artistsWithMetadata);
            }

            return Ok(artists);
        }

        [RequestQueryExists(new string[] { "Fields" })]
        public async Task<IActionResult> GetShapedArtists([FromQuery] RequestParameters requestParameters)
        {
            if (string.IsNullOrWhiteSpace(requestParameters.OrderBy))
            {
                requestParameters.OrderBy = "StageName";
            }

            if (!_typeHelperService.TypeHasProperties<ArtistDto>(requestParameters.Fields))
            {
                return BadRequest();
            }

            var artistPagedList = await GetPagedListOfArtists(requestParameters);

            var paginationMetaData = _controllerHelper.CreatePaginationMetadataObject(artistPagedList, requestParameters, "Getartists");

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paginationMetaData));

            var artists = Mapper.Map<IEnumerable<ArtistDto>>(artistPagedList);

            if (requestParameters.IncludeMetadata)
            {
                var shapedArtists = artists.ShapeData(requestParameters.Fields);
                var artistsWithMetadata = new EntityWithPaginationMetadataDto<ExpandoObject>(paginationMetaData, shapedArtists);
                return Ok(artistsWithMetadata);
            }

            return Ok(artists.ShapeData(requestParameters.Fields));
        }

        [RequestMatchesMediaTypeHeader("Content-Type", new string[] { "application/vnd.BD.json+hateoas" })]
        public async Task<IActionResult> GetArtistsWithHateoas([FromQuery] RequestParameters requestParameters)
        {
            if (string.IsNullOrWhiteSpace(requestParameters.OrderBy))
            {
                requestParameters.OrderBy = "StageName";
            }


            if (!_typeHelperService.TypeHasProperties<ArtistDto>(requestParameters.Fields))
            {
                return BadRequest();
            }

            var artistPagedList = await GetPagedListOfArtists(requestParameters);

            var paginationMetaData = _controllerHelper.CreatePaginationMetadataObject(artistPagedList, requestParameters, "GetArtists");

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paginationMetaData));

            var artists = Mapper.Map<IEnumerable<ArtistDto>>(artistPagedList);

            var shapedArtists = artists.ShapeData(requestParameters.Fields);

            var shapedArtistsWithLinks = shapedArtists.Select(artist => 
            {
                var artistDict = artist as IDictionary<string, object>;
                var linksForArtist =_hateoasHelper.CreateLinksForResource((Guid)artistDict["Id"], requestParameters.Fields, "Artist");

                artistDict.Add("links", linksForArtist);

                return artistDict;
            });

            var links = _hateoasHelper.CreateLinksForResources(requestParameters, artistPagedList.HasNext, artistPagedList.HasPrevious, "Artist");


            if (requestParameters.IncludeMetadata)
            {
                var linkedArtistsWithMetadata = _controllerHelper.CreateLinkedentityWithmetadataObject(paginationMetaData, shapedArtistsWithLinks, links);
                return Ok(linkedArtistsWithMetadata);
            }
            else
            {
                var linkedResourceCollection = new ExpandoObject();
                ((IDictionary<string, object>)linkedResourceCollection).Add("records", shapedArtistsWithLinks);
                ((IDictionary<string, object>)linkedResourceCollection).Add("links", links);
                return Ok(linkedResourceCollection);
            }

        }

        [HttpGet("{artistId}", Name = "GetArtist")]
        [RouteParameterValidationFilter]
        public async Task<IActionResult> GetArtist([FromRoute] Guid artistId, [FromQuery] string fields)
        {

            var artistFromRepo = await _artistRepository.GetArtistAsync(artistId);

            if (artistFromRepo == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrWhiteSpace(fields))
            {
                if (!_typeHelperService.TypeHasProperties<ArtistDto>(fields))
                {
                    return BadRequest();
                }

                if (!_propertyMappingService.ValidMappingExistsFor<ArtistDto, Artist>(fields))
                {
                    return BadRequest();
                }


                var artist = Mapper.Map<ArtistDto>(artistFromRepo);

                return Ok(artist.ShapeData(fields));

            }

            var artistToReturn = Mapper.Map<ArtistDto>(artistFromRepo);
            return Ok(artistToReturn);
        }

        [HttpGet("{artistId}", Name = "GetArtist")]
        [RequestMatchesMediaTypeHeader("Content-Type", new string[] { "application/vnd.BD.json+hateoas" })]
        public async Task<IActionResult> GetArtistWithHateoas( [FromRoute] Guid artistId, [FromQuery] string fields)
        {
            var artistFromRepo = await _artistRepository.GetArtistAsync(artistId);

            if (artistFromRepo == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrWhiteSpace(fields))
            {
                if (!_typeHelperService.TypeHasProperties<ArtistDto>(fields))
                {
                    return BadRequest();
                }

                if (!_propertyMappingService.ValidMappingExistsFor<ArtistDto, Artist>(fields))
                {
                    return BadRequest();
                }
            }

            var artist = Mapper.Map<ArtistDto>(artistFromRepo);
            var shapedArtistToReturn = _controllerHelper.ShapeAndAddLinkToObject(artist, "Artist", fields);
            return Ok(shapedArtistToReturn);
        }

        [HttpPost]
        [ArtistResultFilter]
        public async Task<IActionResult> CreateArtist([FromBody] ArtistForCreationDto artist, [FromHeader] string mediaType)
        {

            var artistEntity = Mapper.Map<Artist>(artist);
            artistEntity.Id = Guid.NewGuid();

            _artistRepository.Create(artistEntity);

            if (!await _artistRepository.SaveChangesAsync())
            {
                _logger.LogError("Creation of an artist failed on saving to database");
            }

            var artistToReturn = Mapper.Map<ArtistDto>(artistEntity);

            if (!string.IsNullOrWhiteSpace(mediaType) && mediaType == "application/vnd.BD.json+hateoas")
            {
                var shapedArtist = _controllerHelper.ShapeAndAddLinkToObject(artistToReturn, "Artist", null);
                return CreatedAtRoute("GetArtist", new { artistId = artistToReturn.Id }, shapedArtist);

            }

            return CreatedAtRoute("GetArtist", new { artistId = artistToReturn.Id }, artistToReturn);
        }

        [HttpPut("{artistId}", Name = "UpdateArtist")]
        [RouteParameterValidationFilter]
        public async Task<IActionResult> UpdateArtist(
            [FromRoute] Guid artistId,
            [FromBody] ArtistForUpdateDto artist,
            [FromHeader] string mediaType)
        {

            var artistFromRepo = await _artistRepository.GetArtistAsync(artistId);
            //  upserting
            if (artistFromRepo == null)
            {
                var artistEntity = Mapper.Map<Artist>(artist);
                artistEntity.Id = artistId;
                _artistRepository.Create(artistEntity);

                if (!await _artistRepository.SaveChangesAsync())
                {
                    _logger.LogError($"Upserting for artist {artistId} failed");
                }

                var artistToReturn = Mapper.Map<ArtistDto>(artistEntity);

                if (!string.IsNullOrWhiteSpace(mediaType) && mediaType == "application/vnd.BD.json+hateoas")
                {
                    var shapedArtist = _controllerHelper.ShapeAndAddLinkToObject(artistToReturn, "Artist", null);
                    return CreatedAtRoute("GetArtist", new { artistId = artistToReturn.Id }, shapedArtist);

                }
                else
                {
                    return CreatedAtRoute("GetArtist", new { artistId = artistToReturn.Id }, artistToReturn);
                }

            }

            Mapper.Map(artist, artistFromRepo);

            _artistRepository.Update(artistFromRepo);

            if (!await _artistRepository.SaveChangesAsync())
            {
                _logger.LogError($"updating for artist {artistId} failed");
            }

            return NoContent();

        }

        [HttpPatch("{artistId}", Name = "PartiallyUpdateArtist")]
        [RouteParameterValidationFilter]
        public async Task<IActionResult> PartiallyUpdateArtist(
            [FromRoute] Guid artistId,
            [FromBody] JsonPatchDocument<ArtistForUpdateDto> patchDoc,
            [FromHeader] string mediaType)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            var artistFromRepo = await _artistRepository.GetArtistAsync(artistId);

            //  upserting
            if (artistFromRepo == null)
            {
                var artistForUpdateDto = new ArtistForUpdateDto();

                patchDoc.ApplyTo(artistForUpdateDto, ModelState);

                if (!TryValidateModel(artistForUpdateDto))
                {
                    return new ErrorProcessingEntityObjectResult(ModelState);
                }

                var artistToAdd = Mapper.Map<Artist>(artistForUpdateDto);
                artistToAdd.Id = artistId;

                _artistRepository.Create(artistToAdd);

                if (!await _artistRepository.SaveChangesAsync())
                {
                    _logger.LogError($"Upserting for artist {artistId} failed");
                }

                var artistToReturn = Mapper.Map<ArtistDto>(artistToAdd);


                if (!string.IsNullOrWhiteSpace(mediaType) && mediaType == "application/vnd.BD.json+hateoas")
                {
                    var shapedArtist = _controllerHelper.ShapeAndAddLinkToObject(artistToReturn, "Artist", null);
                    return CreatedAtRoute("GetArtist", new { artistId = artistToReturn.Id }, shapedArtist);

                }
                else
                {
                    return CreatedAtRoute("GetArtist", new { artistId = artistToReturn.Id }, artistToReturn);
                }

            }

            var artistToPatch = Mapper.Map<ArtistForUpdateDto>(artistFromRepo);

            patchDoc.ApplyTo(artistToPatch, ModelState);

            if (!TryValidateModel(artistToPatch))
            {
                return new ErrorProcessingEntityObjectResult(ModelState);
            }

            Mapper.Map(artistToPatch, artistFromRepo);

            _artistRepository.Update(artistFromRepo);

            if (!await _artistRepository.SaveChangesAsync())
            {
                _logger.LogError($"Updating for artist {artistId} failed");
            }

            return NoContent();
        }

        [HttpDelete("{artistId}", Name = "DeleteArtist")]
        [RouteParameterValidationFilter]
        public async Task<IActionResult> DeleteArtist([FromRoute] Guid artistId)
        {

            var artistFromRepo = await _artistRepository.GetArtistAsync(artistId);

            if (artistFromRepo == null)
            {
                return NotFound();
            }

            _artistRepository.Delete(artistFromRepo);

            if (!await _artistRepository.SaveChangesAsync())
            {
                _logger.LogError($"Deleting for artist {artistId} failed");
            }

            return Ok();
        }

        private async Task<PagedList<Artist>> GetPagedListOfArtists(RequestParameters requestParameters)
        {
            //  change to better design  pattern
            //  consider builder design pattern
            return await _artistRepository.GetArtistsAsync(
                requestParameters.OrderBy, requestParameters.SearchQuery,
                requestParameters.PageNumber, requestParameters.PageSize,
                _propertyMappingService.GetPropertyMapping<ArtistDto, Artist>());
        }

    }
}
