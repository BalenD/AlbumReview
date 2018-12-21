using System.Collections.Generic;

namespace AlbumReview.Services.Web
{
    public interface IPropertyMappingService
    {
        bool validMappingExistsFor<TSource, TDestination>(string fields);
        Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>();
        void AddArtistPropertyMapping<TSource, TDestination>();
        void AddReviewPropertyMapping<TSource, TDestination>();
    }
}
