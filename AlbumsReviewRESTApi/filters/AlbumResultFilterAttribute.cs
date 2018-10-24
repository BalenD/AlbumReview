using AlbumsReviewRESTApi.Entities;
using AlbumsReviewRESTApi.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlbumsReviewRESTApi.filters
{
    public class AlbumResultFilterAttribute : ResultFilterAttribute
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

            if (resultFromControllerAction.Value is IEnumerable<Album>)
            {
                resultFromControllerAction.Value = Mapper.Map<IEnumerable<AlbumDto>>(resultFromControllerAction.Value);
            }
            else
            {
                resultFromControllerAction.Value = Mapper.Map<AlbumDto>(resultFromControllerAction.Value);
            }


            await next();
        }
    }
}
