using System.Collections.Generic;

namespace AlbumReview.Services.Web
{
    class PropertyMapping<TSource, TDestination> : IPropertyMapping
    {
        public Dictionary<string, IEnumerable<string>> _mappingDictionary { get; private set; }

        public PropertyMapping(Dictionary<string, IEnumerable<string>> mappingDictionary)
        {
            _mappingDictionary = mappingDictionary;
        }
    }
}
