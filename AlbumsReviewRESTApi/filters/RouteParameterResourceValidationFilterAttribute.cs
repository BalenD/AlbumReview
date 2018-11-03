using AlbumsReviewRESTApi.Services.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace AlbumsReviewRESTApi.filters
{
    public class RouteParameterResourceValidationFilterAttribute : ActionFilterAttribute
    {
        private IRepository _repository;

        public RouteParameterResourceValidationFilterAttribute(IRepository repository)
        {
            _repository = repository;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.RouteData.Values.ContainsKey("artistId"))
            {
                var artistId = Guid.Parse(context.RouteData.Values["artistId"].ToString());
                if (!_repository.ArtistExists(artistId))
                {
                    context.Result = new NotFoundResult();
                }
            }
            else if (context.RouteData.Values.ContainsKey("albumId"))
            {
                var albumId = Guid.Parse(context.RouteData.Values["artistId"].ToString());
                if (!_repository.AlbumExists(albumId))
                {
                    context.Result = new NotFoundResult();
                }
            }
        }

    }
}
