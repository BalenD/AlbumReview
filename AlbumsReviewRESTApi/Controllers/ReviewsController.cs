using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AlbumsReviewRESTApi.Services.Repositories;
using AlbumsReviewRESTApi.Services;
using AlbumsReviewRESTApi.filters;
using AlbumsReviewRESTApi.Models;
using AlbumsReviewRESTApi.Entities;
using AlbumsReviewRESTApi.Helpers;
using AutoMapper;
using System.Dynamic;
using Microsoft.AspNetCore.JsonPatch;

namespace AlbumsReviewRESTApi.Controllers
{
    [Route("api/artists/{artistId}/albums/{albumId}/reviews")]
    public class ReviewsController : Controller
    {
        private IReviewRepository _reviewRepository;
        private IPropertyMappingService _propertyMappingService;
        private ITypeHelperService _typeHelperService;
        private IUrlHelper _urlHelper;

        public ReviewsController(IReviewRepository reviewRepository, IPropertyMappingService propertyMappingService,
                                 ITypeHelperService typeHelperService, IUrlHelper urlHelper)
        {
            _reviewRepository = reviewRepository;
            _propertyMappingService = propertyMappingService;
            _typeHelperService = typeHelperService;
            _urlHelper = urlHelper;
        }

        [HttpGet( Name = "GetReviewsForAlbum")]
        public async Task<IActionResult> GetReviewsForAlbum([FromRoute]Guid albumId, RequestParameters reviewRequestParameters)
        {
            if (string.IsNullOrWhiteSpace(reviewRequestParameters.OrderBy))
            {
                reviewRequestParameters.OrderBy = "Submitted";
            }
            if (!_propertyMappingService.validMappingExistsFor<ReviewDto, Review>(reviewRequestParameters.Fields))
            {
                return BadRequest();
            }

            if (!_typeHelperService.TypeHasProperties<ReviewDto>(reviewRequestParameters.Fields))
            {
                return BadRequest();
            }
            var reviewsForAlbumPagedList = await _reviewRepository.GetReviewsForAlbumAsync(albumId, reviewRequestParameters);

            var previousPageLink = reviewsForAlbumPagedList.HasPrevious ? CreateUrlForReviewResource(reviewRequestParameters, PageType.PreviousPage) : null;
            var nextPageLink = reviewsForAlbumPagedList.HasNext ? CreateUrlForReviewResource(reviewRequestParameters, PageType.NextPage) : null;

            var paginationMetaData = new PaginationMetadata()
            {
                TotalCount = reviewsForAlbumPagedList.TotalCount,
                PageSize = reviewsForAlbumPagedList.PageSize,
                CurrentPage = reviewsForAlbumPagedList.CurrentPage,
                TotalPages = reviewsForAlbumPagedList.TotalPages,
                PreviousPageLink = previousPageLink,
                NextPageLink = nextPageLink
            };


            Response.Headers.Add("X-Pagination", Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetaData));

            var reviews = Mapper.Map<IEnumerable<ReviewDto>>(reviewsForAlbumPagedList);

            if (reviewRequestParameters.IncludeMetadata)
            {
                var records = reviews.ShapeData(reviewRequestParameters.Fields);

                var artistsWithMetadata = new EntityWithPaginationMetadataDto<ExpandoObject>(paginationMetaData, records);
                return Ok(artistsWithMetadata);
            }

            return Ok(reviews.ShapeData(reviewRequestParameters.Fields));
        }

        [HttpGet("{id}", Name = "GetReviewForAlbum")]
        public async Task<IActionResult> GetReviewForAlbum([FromRoute] Guid albumId, [FromRoute] Guid id, [FromQuery] string fields)
        {
            if (albumId == Guid.Empty)
            {
                return BadRequest();
            }

            if (id == Guid.Empty)
            {
                return BadRequest();
            }

            var reviewForAlbumFromRepo = await _reviewRepository.GetReviewForAlbumAsync(albumId, id);

            if (reviewForAlbumFromRepo == null)
            {
                return NotFound();
            }

            if (fields != null)
            {
                if (!_propertyMappingService.validMappingExistsFor<ReviewDto, Review>(fields))
                {
                    return BadRequest();
                }

                if (!_typeHelperService.TypeHasProperties<ReviewDto>(fields))
                {
                    return BadRequest();
                }

                var review = Mapper.Map<ReviewDto>(reviewForAlbumFromRepo);

                return Ok(review.ShapeData(fields));
            }

            var reviewToReturn = Mapper.Map<ReviewDto>(reviewForAlbumFromRepo);
            return Ok(reviewToReturn);
        }

        [HttpPost]
        public async Task<IActionResult> CreateReviewForAlbum([FromRoute] Guid albumId, [FromBody] ReviewForCreationDto reviewToCreate)
        {
            if (albumId == Guid.Empty)
            {
                return BadRequest();
            }

            if (reviewToCreate == null)
            {
                return BadRequest();
            }

            if(!ModelState.IsValid)
            {
                return new ErrorProcessingEntityObjectResult(ModelState);
            }

            var reviewToCreateEntity = Mapper.Map<Review>(reviewToCreate);

            await _reviewRepository.AddReviewForAlbumAsync(albumId, reviewToCreateEntity);

            if (!await _reviewRepository.SaveChangesAsync())
            {
                throw new Exception($"adding review to album {albumId} failed");
            }
            return CreatedAtRoute("GetReviewForAlbum", new { albumId =  albumId, id = reviewToCreateEntity.Id }, reviewToCreateEntity);

        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateReviewForAlbum([FromRoute] Guid albumId, [FromRoute] Guid id, [FromBody] ReviewForUpdateDto reviewToUpdate)
        {
            if (albumId == Guid.Empty)
            {
                return BadRequest();
            }

            if (id == Guid.Empty)
            {
                return BadRequest();
            }

            if (reviewToUpdate == null)
            {
                return BadRequest();
            }

            var reviewFromRepo = await _reviewRepository.GetReviewForAlbumAsync(albumId, id);

            //  upserting

            if (reviewFromRepo == null)
            {
                var reviewEntity = Mapper.Map<Review>(reviewToUpdate);
                reviewEntity.Id = id;

                await _reviewRepository.AddReviewForAlbumAsync(albumId, reviewEntity);

                if (!await _reviewRepository.SaveChangesAsync())
                {
                    throw new Exception($"upserting review {id} for album {albumId} failed on save");
                }

                var reviewToReturn = Mapper.Map<ReviewDto>(reviewEntity);

                return CreatedAtRoute("GetReviewForAlbum", new { albumId, id = reviewToReturn.Id }, reviewToReturn);
            }

            Mapper.Map(reviewToUpdate, reviewFromRepo);

            _reviewRepository.UpdateReviewForAlbum(albumId, reviewFromRepo);

            if (!await _reviewRepository.SaveChangesAsync())
            {
                throw new Exception($"Updating review {id} for album {albumId} failed on save");
            }

            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PartiallyUpdateReviewForAlbum([FromRoute] Guid albumId, [FromRoute] Guid id, [FromBody] JsonPatchDocument<ReviewForUpdateDto> patchDoc)
        {
            if (albumId == Guid.Empty)
            {
                return BadRequest();
            }

            if (id == Guid.Empty)
            {
                return BadRequest();
            }

            if (patchDoc == null)
            {
                return BadRequest();
            }

            //  TODO: check if artist/album exist first

            var reviewFromRepo = await _reviewRepository.GetReviewForAlbumAsync(albumId, id);

            //  upserting

            if (reviewFromRepo == null)
            {
                var reviewForUpdateDto = new ReviewForUpdateDto();


                patchDoc.ApplyTo(reviewForUpdateDto, ModelState);

                TryValidateModel(reviewForUpdateDto);

                if (!ModelState.IsValid)
                {
                    return new ErrorProcessingEntityObjectResult(ModelState);
                }

                var reviewToAdd = Mapper.Map<Review>(reviewForUpdateDto);

                reviewToAdd.Id = id;

                await _reviewRepository.AddReviewForAlbumAsync(albumId, reviewToAdd);

                if (!await _reviewRepository.SaveChangesAsync())
                {
                    throw new Exception($"upserting review {id} for album {albumId} failed on save");
                }

                var reviewToReturn = Mapper.Map<ReviewDto>(reviewToAdd);

                return CreatedAtRoute("GetReviewForAlbum", new { albumId, id = reviewToReturn.Id }, reviewToReturn);

            }

            var reviewToPatch = Mapper.Map<ReviewForUpdateDto>(reviewFromRepo);

            patchDoc.ApplyTo(reviewToPatch, ModelState);

            TryValidateModel(reviewToPatch);

            if (!ModelState.IsValid)
            {
                return new ErrorProcessingEntityObjectResult(ModelState);
            }

            Mapper.Map(reviewToPatch, reviewFromRepo);

            _reviewRepository.UpdateReviewForAlbum(albumId, reviewFromRepo);

            if (!await _reviewRepository.SaveChangesAsync())
            {
                throw new Exception($"Updating review {id} for album {albumId} failed on save");
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReviewForAlbum([FromRoute] Guid albumId, [FromRoute] Guid id)
        {
            if (albumId == Guid.Empty)
            {
                return BadRequest();
            }

            if (albumId == Guid.Empty)
            {
                return BadRequest();
            }

            var reviewFromRepo = await _reviewRepository.GetReviewForAlbumAsync(albumId, id);

            if (reviewFromRepo == null)
            {
                return NotFound();
            }

            _reviewRepository.DeleteReviewForAlbum(reviewFromRepo);

            if (!await _reviewRepository.SaveChangesAsync())
            {
                throw new Exception($"deleting review {id} for album {albumId} failed on save");
            }

            return Ok();
        }

        private string CreateUrlForReviewResource(RequestParameters artistsRequestParameter, PageType pageType)
        {
            switch (pageType)
            {
                case PageType.PreviousPage:
                    return _urlHelper.Link("GetReviewsForAlbum", new
                    {
                        fields = artistsRequestParameter.Fields,
                        orderBy = artistsRequestParameter.OrderBy,
                        searchQuery = artistsRequestParameter.SearchQuery,
                        pageNumber = artistsRequestParameter.PageNumber - 1,
                        pageSize = artistsRequestParameter.PageSize

                    });
                case PageType.NextPage:
                    return _urlHelper.Link("GetReviewsForAlbum", new
                    {
                        fields = artistsRequestParameter.Fields,
                        orderBy = artistsRequestParameter.OrderBy,
                        searchQuery = artistsRequestParameter.SearchQuery,
                        pageNumber = artistsRequestParameter.PageNumber + 1,
                        pageSize = artistsRequestParameter.PageSize
                    });
                default:
                    return _urlHelper.Link("GetReviewsForAlbum", new
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
