using System.Collections.Generic;

namespace AlbumReview.Services.Web
{
    public interface IPropertyMappingService
    {
        bool ValidMappingExistsFor<TSource, TDestination>(string fields);
        Dictionary<string, IEnumerable<string>> GetPropertyMapping<TSource, TDestination>();
        void AddPropertyMapping<TSource, TDestination>(Dictionary<string, IEnumerable<string>> mapping);
    }
}
