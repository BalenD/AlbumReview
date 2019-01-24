using AlbumReview.Web.Api.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace AlbumReview.Web.Api.services
{
    /// <summary>
    /// Class to help create pagination urls
    /// </summary>
    public class PaginationUrlHelper : IPaginationUrlHelper
    {
        private readonly IUrlHelper _urlHelper;

        public PaginationUrlHelper(IUrlHelper urlHelper)
        {
            _urlHelper = urlHelper;
        }
        /// <summary>
        /// Creates a url based on which page for a resource
        /// </summary>
        /// <param name="requestParameter">Request specific parameters in url (query string)</param>
        /// <param name="pageType">the type of page to create</param>
        /// <param name="routeName">name of the route to create</param>
        /// <returns>a url string with all the pagination data in the query string</returns>
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
                case PageType.Current:
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
