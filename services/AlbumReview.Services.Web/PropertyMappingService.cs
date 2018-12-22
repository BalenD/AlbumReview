using System;
using System.Collections.Generic;
using System.Linq;

namespace AlbumReview.Services.Web
{
    public class PropertyMappingService : IPropertyMappingService
    {

        private IList<IPropertyMapping> propertyMappings = new List<IPropertyMapping>();
        
        public void AddPropertyMapping<TSource, TDestination>(Dictionary<string, IEnumerable<string>> mapping)
        {
            propertyMappings.Add(new PropertyMapping<TSource, TDestination>(mapping));
            
        }

        public Dictionary<string, IEnumerable<string>> GetPropertyMapping<TSource, TDestination>()
        {
            var matchingMapping = propertyMappings.OfType<PropertyMapping<TSource, TDestination>>();

            if (matchingMapping.Count() == 1)
            {
                return matchingMapping.First()._mappingDictionary;
            }

            throw new Exception($"Can't find property mapping instance for <{typeof(TSource)}>");
        }

        public bool ValidMappingExistsFor<TSource, TDestination>(string fields)
        {
            var propertyMapping = GetPropertyMapping<TSource, TDestination>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                return true;
            }

            //  split the strong by ","
            var fieldsAfterSplit = fields.Split(",");

            foreach (var field in fieldsAfterSplit)
            {
                //  trim
                var trimmedField = field.Trim();

                //  remove everything after the first " "
                //  if the fields are coming from an orderBy string
                //  this part must be ignored
                var indexOfFirstSpace = trimmedField.IndexOf(" ");
                var propertyName = indexOfFirstSpace == -1 ? trimmedField : trimmedField.Remove(indexOfFirstSpace);

                //  find the matching property
                if (!propertyMapping.ContainsKey(propertyName))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
