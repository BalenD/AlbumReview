using System;
using System.Collections.Generic;
using AlbumReview.Web.Api.DtoModels;
using AlbumReview.Web.Api.Helpers;

namespace AlbumReview.Web.Api.services
{
    public interface IHateoasHelper
    {
        IEnumerable<LinkDto> CreateLinksForResource(Guid id, string fields, string resourceName);
        IEnumerable<LinkDto> CreateLinksForResources(RequestParameters requestParameters, bool hasNext, bool hasPrevious, string resourceName, object values = null);
        IEnumerable<LinkDto> CreateLinksForChildResources(string resourceName, object values);
    }
}