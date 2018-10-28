using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AlbumsReviewRESTApi.Services.Repositories;
using AlbumsReviewRESTApi.filters;

namespace AlbumsReviewRESTApi.Controllers
{
    [Route("api/artists/{artistId}/albums/{albumId}/reviews")]
    public class ReviewsController : Controller
    {
        private IReviewRepository _reviewRepository;

        public ReviewsController(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        [ReviewResultFilter]
        public async Task<IActionResult> GetReviews([FromRoute]Guid albumId)
        {
            var reviewsForAlbum = await _reviewRepository.GetReviewsForAlbumAsync(albumId);
            return Ok();
        }

        [ReviewResultFilter]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetReview([FromRoute] Guid albumId, [FromRoute] Guid id)
        {
            var reviewForAlbum = await _reviewRepository.GetReviewForAlbum(albumId, id);

            return Ok(reviewForAlbum);
        }
    }
}
