﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;
using System.Threading.Tasks;
using AlbumReview.Data.Models;
using AlbumReview.Web.Api.DtoModels;

namespace AlbumReview.Web.Api.filters
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
