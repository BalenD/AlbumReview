using AlbumReview.Services.Data.helpers;
using AlbumReview.Web.Api.DtoModels;
using AlbumReview.Web.Api.Helpers;
using System.Collections.Generic;
using System.Dynamic;

namespace AlbumReview.Web.Api.services
{
    public interface IControllerHelper
    {
        ExpandoObject CreateLinkedentityWithmetadataObject<T>(PaginationMetadata paginationMetadata, IEnumerable<T> objs, IEnumerable<LinkDto> links);
        ExpandoObject ShapeAndAddLinkToObject<T>(T obj, string resourceName, string field);
        PaginationMetadata CreatePaginationMetadataObject<T>(PagedList<T> pagedlist, RequestParameters requestParameters, string routeName);
    }
}
