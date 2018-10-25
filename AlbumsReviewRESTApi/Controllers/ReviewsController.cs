using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AlbumsReviewRESTApi.Services;

namespace AlbumsReviewRESTApi.Controllers
{
    [Route("api/artists/{artistId}/albums/{albumId}/reviews")]
    public class ReviewsController : Controller
    {
        private IAlbumsReviewRepository _albumsReviewRepository;

        public ReviewsController(IAlbumsReviewRepository albumsReviewRepository)
        {
            _albumsReviewRepository = albumsReviewRepository;
        }

        public async Task<IActionResult> GetReviews([FromRoute]Guid albumId)
        {
            var reviewsForAlbum = await _albumsReviewRepository.GetReviewsAsync(albumId);
            return Ok(reviewsForAlbum);
        }
    }
}
