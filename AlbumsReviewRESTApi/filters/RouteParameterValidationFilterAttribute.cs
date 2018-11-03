using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace AlbumsReviewRESTApi.filters
{
    public class RouteParameterValidationFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {

            if (context.RouteData.Values.ContainsKey("artistId"))
            {
                if (!Guid.TryParse(context.RouteData.Values["artistId"].ToString(), out Guid artistId))
                {
                    context.Result = new BadRequestResult();
                }

                if (artistId == Guid.Empty)
                {
                    context.Result = new BadRequestResult();
                }
            }

            if (context.RouteData.Values.ContainsKey("albumId"))
            {
                if (!Guid.TryParse(context.RouteData.Values["albumId"]?.ToString(), out Guid albumId))
                {
                    context.Result = new BadRequestResult();
                }

                if (albumId == Guid.Empty)
                {
                    context.Result = new BadRequestResult();
                }
            }

            if (context.RouteData.Values.ContainsKey("reviewId"))
            {
                if (!Guid.TryParse(context.RouteData.Values["albumId"]?.ToString(), out Guid albumId))
                {
                    context.Result = new BadRequestResult();
                }

                if (albumId == Guid.Empty)
                {
                    context.Result = new BadRequestResult();
                }
            }
           
        }
    }
}
