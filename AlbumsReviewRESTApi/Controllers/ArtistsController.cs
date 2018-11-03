using AlbumsReviewRESTApi.Entities;
using AlbumsReviewRESTApi.filters;
using AlbumsReviewRESTApi.Services.Repositories;
using Microsoft.AspNetCore.JsonPatch;
using AlbumsReviewRESTApi.Helpers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using AlbumsReviewRESTApi.Models;
using AutoMapper;
using AlbumsReviewRESTApi.Services;
using System.Collections.Generic;
using System.Dynamic;

namespace AlbumsReviewRESTApi.Controllers
{
    
    [Route("api/artists")]
    public class ArtistsController : Controller
    {
        private IArtistRepository _artistRepository;
        private IPropertyMappingService _propertyMappingService;
        private ITypeHelperService _typeHelperService;
        private IUrlHelper _urlHelper;

        public ArtistsController(IArtistRepository artistRepository, IPropertyMappingService propertyMappingService,
                                 ITypeHelperService typeHelperService, IUrlHelper urlHelper)
        {
            _artistRepository = artistRepository;
            _propertyMappingService = propertyMappingService;
            _typeHelperService = typeHelperService;
            _urlHelper = urlHelper;
        }

        
        [HttpGet(Name = "GetArtists")]
        public async Task<IActionResult> GetArtists(RequestParameters artistsRequestParameters)
        {
            if (string.IsNullOrWhiteSpace(artistsRequestParameters.OrderBy))
            {
                artistsRequestParameters.OrderBy = "StageName";
            }

            if (!_propertyMappingService.validMappingExistsFor<ArtistDto, Artist>(artistsRequestParameters.Fields))
            {
                return BadRequest();
            }

            if (!_typeHelperService.TypeHasProperties<ArtistDto>(artistsRequestParameters.Fields))
            {
                return BadRequest();
            }


            var artistPagedList = await _artistRepository.GetArtistsAsync(artistsRequestParameters);

            var previousPageLink = artistPagedList.HasPrevious ? CreateUrlForArtistResource(artistsRequestParameters, PageType.PreviousPage) : null;
            var nextPageLink = artistPagedList.HasNext ? CreateUrlForArtistResource(artistsRequestParameters, PageType.NextPage) : null;

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
                if (!_propertyMappingService.validMappingExistsFor<ArtistDto, Artist>(fields))
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


            if (!ModelState.IsValid)
            {
                return new ErrorProcessingEntityObjectResult(ModelState);
            }

            var artistEntity = Mapper.Map<Artist>(artist);

            _artistRepository.AddArtist(artistEntity);

            if (!await _artistRepository.SaveChangesAsync())
            {
                throw new Exception("creating an artist failed on save");
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
                _artistRepository.AddArtist(artistEntity);

                if (!await _artistRepository.SaveChangesAsync())
                {
                    throw new Exception($"Upserting for artist {artistId} failed");
                }

                var artistToReturn = Mapper.Map<ArtistDto>(artistEntity);

                return CreatedAtRoute("GetArtist", new { artistId = artistToReturn.Id }, artistToReturn);

            }

            Mapper.Map(artist, artistFromRepo);

            _artistRepository.UpdateArtist(artistFromRepo);

            if (!await _artistRepository.SaveChangesAsync())
            {
                throw new Exception($"updating for artist {artistId} failed");
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

                _artistRepository.AddArtist(artistToAdd);

                if (!await _artistRepository.SaveChangesAsync())
                {
                    throw new Exception($"Upserting for artist {artistId} failed");
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

            _artistRepository.UpdateArtist(artistFromRepo);

            if (!await _artistRepository.SaveChangesAsync())
            {
                throw new Exception($"updating for artist {artistId} failed");
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

            _artistRepository.DeleteArtist(artistFromRepo);

            if (!await _artistRepository.SaveChangesAsync())
            {
                throw new Exception($"deleting for artist {artistId} failed");
            }

            return Ok();
        }


        private string CreateUrlForArtistResource(RequestParameters artistsRequestParameter, PageType pageType)
        {
            switch (pageType)
            {
                case PageType.PreviousPage:
                    return _urlHelper.Link("GetArtists", new
                    {
                        fields = artistsRequestParameter.Fields,
                        orderBy = artistsRequestParameter.OrderBy,
                        searchQuery = artistsRequestParameter.SearchQuery,
                        pageNumber = artistsRequestParameter.PageNumber - 1,
                        pageSize = artistsRequestParameter.PageSize

                    });
                case PageType.NextPage:
                    return _urlHelper.Link("GetArtists", new
                    {
                        fields = artistsRequestParameter.Fields,
                        orderBy = artistsRequestParameter.OrderBy,
                        searchQuery = artistsRequestParameter.SearchQuery,
                        pageNumber = artistsRequestParameter.PageNumber + 1,
                        pageSize = artistsRequestParameter.PageSize
                    });
                default:
                    return _urlHelper.Link("GetArtists", new
                    {
                        fields = artistsRequestParameter.Fields,
                        orderBy = artistsRequestParameter.OrderBy,
                        searchQuery = artistsRequestParameter.SearchQuery,
                        pageNumber = artistsRequestParameter.PageNumber,
                        pageSize = artistsRequestParameter.PageSize
                    });
            }
        }
    }
}
