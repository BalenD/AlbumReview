using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlbumsReviewRESTApi.Helpers
{
    public class ArtistsRequestParameters
    {
        const int maxPageSize = 20;
        private int _pageSize = 1;
        
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
        public string OrderBy { get; set; } = "StageName";
        public string Fields { get; set; }
        public string SearchQuery { get; set; }
        public bool IncludeMetadata { get; set; }
    }
}
