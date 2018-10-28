using System.Collections.Generic;

namespace AlbumsReviewRESTApi.Services
{
    public interface IPropertyMappingService
    {
        bool validMappingExistsFor<TSource, TDestination>(string fields);
        Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>();
    }
}
