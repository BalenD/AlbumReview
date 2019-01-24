namespace AlbumReview.Web.Api.Helpers
{
    public class RequestParameters
    {
        const int maxPageSize = 20;
        private int _pageSize = 10;
        
        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = (value > maxPageSize) ? maxPageSize : value;
            }
        }

        public int PageNumber { get; set; } = 1;
        public string OrderBy { get; set; }
        public string Fields { get; set; }
        public string SearchQuery { get; set; }
        public bool IncludeMetadata { get; set; }
    }
}
