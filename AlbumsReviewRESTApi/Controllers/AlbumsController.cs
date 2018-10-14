using AlbumsReviewRESTApi.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlbumsReviewRESTApi.Controllers
{
    [Route("api/albums")]
    public class AlbumsController : Controller
    {
        private IAlbumsReviewRepository _albumsReviewRepository;

        public AlbumsController(IAlbumsReviewRepository albumsReviewRepository)
        {
            _albumsReviewRepository = albumsReviewRepository;
        }

    }
}
