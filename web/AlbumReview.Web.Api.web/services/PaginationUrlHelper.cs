using AlbumReview.Web.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace AlbumReview.Web.Api.services
{
    public class PaginationUrlHelper : IPaginationUrlHelper
    {
        private readonly IUrlHelper _urlHelper;

        public PaginationUrlHelper(IUrlHelper urlHelper)
        {
            _urlHelper = urlHelper;
        }

        public string CreateUrlForResource(RequestParameters requestParameter, PageType pageType, string routeName)
        {
            switch (pageType)
            {
                case PageType.PreviousPage:
                    return _urlHelper.Link(routeName, new
                    {
                        fields = requestParameter.Fields,
                        orderBy = requestParameter.OrderBy,
                        searchQuery = requestParameter.SearchQuery,
                        pageNumber = requestParameter.PageNumber - 1,
                        pageSize = requestParameter.PageSize

                    });
                case PageType.NextPage:
                    return _urlHelper.Link(routeName, new
                    {
                        fields = requestParameter.Fields,
                        orderBy = requestParameter.OrderBy,
                        searchQuery = requestParameter.SearchQuery,
                        pageNumber = requestParameter.PageNumber + 1,
                        pageSize = requestParameter.PageSize
                    });
                default:
                    return _urlHelper.Link(routeName, new
                    {
                        fields = requestParameter.Fields,
                        orderBy = requestParameter.OrderBy,
                        searchQuery = requestParameter.SearchQuery,
                        pageNumber = requestParameter.PageNumber,
                        pageSize = requestParameter.PageSize
                    });
            }
        }
    }
}
