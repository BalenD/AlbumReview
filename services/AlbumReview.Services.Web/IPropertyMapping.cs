using System.Collections.Generic;

namespace AlbumReview.Services.Web
{
    public interface IPropertyMapping
    {
        Dictionary<string, IEnumerable<string>> _mappingDictionary { get; }
    }
}
