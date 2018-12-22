using AlbumReview.Web.filters;
using Microsoft.AspNetCore.JsonPatch;
using AlbumReview.Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using AutoMapper;
using System.Collections.Generic;
using System.Dynamic;
using AlbumReview.Services.Web;
using AlbumReview.Services.Data;
using AlbumReview.Data.Models;
using AlbumReview.Web.DtoModels;
using Microsoft.Extensions.Logging;
using AlbumReview.Web.Api.services;

namespace AlbumReview.Web.Controllers
{

    [Route("api/artists")]
    [ApiController]
    public class ArtistsController : Controller
    {
        private IArtistRepository _artistRepository;
        private IPropertyMappingService _propertyMappingService;
        private ITypeHelperService _typeHelperService;
        private readonly IUrlHelper _urlHelper;
        private readonly ILogger<ArtistsController> _logger;
        private readonly IPaginationUrlHelper _paginationUrlHelper;

        public ArtistsController(
            IArtistRepository artistRepository, IPropertyMappingService propertyMappingService,
            ITypeHelperService typeHelperService, IUrlHelper urlHelper, ILogger<ArtistsController> logger,
            IPaginationUrlHelper paginationUrlHelper)
        {
            _artistRepository = artistRepository;
            _typeHelperService = typeHelperService;
            _urlHelper = urlHelper;
            _logger = logger;
            _paginationUrlHelper = paginationUrlHelper;
            _propertyMappingService = propertyMappingService;
        }

        
        [HttpGet(Name = "GetArtists")]
        public async Task<IActionResult> GetArtists([FromQuery] RequestParameters artistsRequestParameters)
        {
            if (string.IsNullOrWhiteSpace(artistsRequestParameters.OrderBy))
            {
                artistsRequestParameters.OrderBy = "StageName";
            }

            if (!string.IsNullOrWhiteSpace(artistsRequestParameters.Fields))
            {

                if (!_typeHelperService.TypeHasProperties<ArtistDto>(artistsRequestParameters.Fields))
                {
                    return BadRequest();
                }
            }



            var artistPagedList = await _artistRepository.GetArtistsAsync(
                artistsRequestParameters.OrderBy, artistsRequestParameters.SearchQuery,
                artistsRequestParameters.PageNumber, artistsRequestParameters.PageSize,
                _propertyMappingService.GetPropertyMapping<ArtistDto, Artist>());

            var previousPageLink = artistPagedList.HasPrevious ? _paginationUrlHelper.CreateUrlForResource(artistsRequestParameters, PageType.PreviousPage, "GetArtists") : null;
            var nextPageLink = artistPagedList.HasNext ? _paginationUrlHelper.CreateUrlForResource(artistsRequestParameters, PageType.NextPage, "GetArtists") : null;

            var paginationMetaData = new PaginationMetadata()
            {
                TotalCount = artistPagedList.TotalCount,
                PageSize = artistPagedList.PageSize,
                CurrentPage = artistPagedList.CurrentPage,
                TotalPages = artistPagedList.TotalPages,
                PreviousPageLink = previousPageLink,
                NextPageLink = nextPageLink
            };


            Response.Headers.Add("X-Pagination", Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetaData));

            var artists = Mapper.Map<IEnumerable<ArtistDto>>(artistPagedList);

            if (artistsRequestParameters.IncludeMetadata)
            {
                var records = artists.ShapeData(artistsRequestParameters.Fields);

                var artistsWithMetadata = new EntityWithPaginationMetadataDto<ExpandoObject>(paginationMetaData, records);
                return Ok(artistsWithMetadata);
            }
            
            return Ok(artists.ShapeData(artistsRequestParameters.Fields));
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

            if (fields != null)
            {
                if (!_propertyMappingService.ValidMappingExistsFor<ArtistDto, Artist>(fields))
                {
                    return BadRequest();
                }

                if (!_typeHelperService.TypeHasProperties<ArtistDto>(fields))
                {
                    return BadRequest();
                }

                var artist = Mapper.Map<ArtistDto>(artistFromRepo);

                return Ok(artist.ShapeData(fields));

            }

            var artistToReturn = Mapper.Map<ArtistDto>(artistFromRepo);
            return Ok(artistToReturn);
        }

        [HttpPost]
        [ArtistResultFilter]
        public async Task<IActionResult> CreateArtist([FromBody] ArtistForCreationDto artist)
        {
            if (artist == null)
            {
                return BadRequest();
            }
            
            TryValidateModel(artist);

            if (artist.Id == Guid.Empty)
            {
                artist.Id = Guid.NewGuid();
            }

            if (!ModelState.IsValid)
            {
                return new ErrorProcessingEntityObjectResult(ModelState);
            }

            var artistEntity = Mapper.Map<Artist>(artist);

            _artistRepository.Create(artistEntity);

            if (!await _artistRepository.SaveChangesAsync())
            {
                _logger.LogError("Creation of an artist failed on saving to database");
            }

            return CreatedAtRoute("GetArtist", new { artistId = artist.Id }, artistEntity);
        }

        [HttpPut("{artistId}")]
        [RouteParameterValidationFilter]
        public async Task<IActionResult> UpdateArtist([FromRoute] Guid artistId, [FromBody] ArtistForUpdateDto artist)
        {
            if (artist == null)
            {
                return BadRequest();
            }

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

                return CreatedAtRoute("GetArtist", new { artistId = artistToReturn.Id }, artistToReturn);

            }

            Mapper.Map(artist, artistFromRepo);

            _artistRepository.Update(artistFromRepo);

            if (!await _artistRepository.SaveChangesAsync())
            {
                _logger.LogError($"updating for artist {artistId} failed");
            }

            return NoContent();

        }

        [HttpPatch("{artistId}")]
        [RouteParameterValidationFilter]
        public async Task<IActionResult> PartiallyUpdateArtist([FromRoute] Guid artistId, [FromBody] JsonPatchDocument<ArtistForUpdateDto> patchDoc)
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

                TryValidateModel(artistForUpdateDto);

                if (!ModelState.IsValid)
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

                return CreatedAtRoute("GetArtist", new { artistId = artistToReturn.Id }, artistToReturn);

            }

            var artistToPatch = Mapper.Map<ArtistForUpdateDto>(artistFromRepo);

            patchDoc.ApplyTo(artistToPatch, ModelState);

            TryValidateModel(artistToPatch);

            if (!ModelState.IsValid)
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

        [HttpDelete("{artistId}")]
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

    }
}
