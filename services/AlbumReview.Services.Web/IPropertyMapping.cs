using System.Collections.Generic;

namespace AlbumReview.Services.Web
{
    public interface IPropertyMapping
    {
        Dictionary<string, PropertyMappingValue> _mappingDictionary { get; }
    }
}
