using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using AlbumReview.Services.Web;

namespace AlbumReview.Services.Data.helpers
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> ApplySort<T>(this IQueryable<T> source, string orderBy, Dictionary<string, PropertyMappingValue> mappingDictionary)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (mappingDictionary == null)
            {
                throw new ArgumentNullException("mappingDictionary");
            }

            if (string.IsNullOrWhiteSpace(orderBy))
            {
                return source;
            }

            // split the string by comma
            var orderByAfterSplit = orderBy.Split(",");

            //  apply each orderby clause in reverse order
            //  otherwise the IQueryable will be ordered in the wrong order
            foreach (var orderByClause in orderByAfterSplit.Reverse())
            {
                //  trim orderByClause, as it might contain leading or trailing spaces
                var trimmedOrderByClause = orderByClause.Trim();

                //  if the sort option ends with " desc", order by descending, otherwise ascending
                var orderDescending = trimmedOrderByClause.EndsWith(" desc");

                //  remove the " asc" or " desc" from the orderByClause
                //  so we get the property name to look for in the mapping dictionary
                var indexOfFirstSpace = trimmedOrderByClause.IndexOf(" ");
                var propertyName = indexOfFirstSpace == -1 ? trimmedOrderByClause : trimmedOrderByClause.Remove(indexOfFirstSpace);


                //  find the matching property
                if (!mappingDictionary.ContainsKey(propertyName))
                {
                    throw new ArgumentException($"key mapping for {propertyName} is missing");
                }

                //  get the PropertyMappingValue
                var propertyMappingValue = mappingDictionary[propertyName];

                if (propertyMappingValue == null)
                {
                    throw new ArgumentNullException("propertyMappingValue");
                }

                //  run through the property names in reverse
                //  so the orderby clauses are applied in the correct order
                foreach (var destinationProperty in propertyMappingValue.DestinationProperties.Reverse())
                {
                    //  revert sort order if neccesary
                    if (propertyMappingValue.Revert)
                    {
                        orderDescending = !orderDescending;
                    }
                    
                    source = source.OrderBy(destinationProperty + (orderDescending ? " descending" : " ascending"));
                }

            }
            return source;
        }
    }
}
