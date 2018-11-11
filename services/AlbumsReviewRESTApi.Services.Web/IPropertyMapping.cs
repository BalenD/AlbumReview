using System.Collections.Generic;

namespace AlbumsReviewRESTApi.Services.Web
{
    public interface IPropertyMapping
    {
        Dictionary<string, PropertyMappingValue> _mappingDictionary { get; }
    }
}
