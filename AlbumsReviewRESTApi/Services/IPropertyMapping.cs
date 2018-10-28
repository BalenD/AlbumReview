using System.Collections.Generic;

namespace AlbumsReviewRESTApi.Services
{
    public interface IPropertyMapping
    {
        Dictionary<string, PropertyMappingValue> _mappingDictionary { get; }
    }
}