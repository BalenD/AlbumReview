using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;
using System.Threading.Tasks;
using AlbumsReviewRESTApi.Data.Models;
using AlbumsReviewRESTApi.Services.Data.DtoModels;

namespace AlbumsReviewRESTApi.Web.filters
{
    public class ReviewResultFilterAttribute : ResultFilterAttribute
    {
        public async override Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            //  get the result returned from the controller action
            var resultFromControllerAction = context.Result as ObjectResult;

            if (resultFromControllerAction?.Value == null
                || resultFromControllerAction.StatusCode < 200
                || resultFromControllerAction.StatusCode >= 300)
            {
                await next();
                return;
            }

            if (resultFromControllerAction.Value is IEnumerable<Review>)
            {
                resultFromControllerAction.Value = Mapper.Map<IEnumerable<ReviewDto>>(resultFromControllerAction.Value);
            }
            else
            {
                resultFromControllerAction.Value = Mapper.Map<ReviewDto>(resultFromControllerAction.Value);
            }


            await next();
        }

    }
}
