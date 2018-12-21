using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;

namespace AlbumReview.Web.Helpers
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<ExpandoObject> ShapeData<TSource>(this IEnumerable<TSource> source, string fields)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            //  create a list to hold ExpandoObjects
            var expandoObjectList = new List<ExpandoObject>();

            //  create a list with propertyinfo objects on TSoruce
            //  reflection is expensive
            //  do it once nad reuse the results
            //  part of the reflection is on the type of the object (tsource) not on the instance
            var propertyInfoList = new List<PropertyInfo>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                //  all public properties should be in the ExpandoObject
                var propertyInfos = typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                propertyInfoList.AddRange(propertyInfos);
            }
            else
            {
                //  only the public properties that match the fields should be in the ExpandoObject

                //  split the coma seperated fields
                var fieldsAfterSplit = fields.Split(",");

                foreach (var field in fieldsAfterSplit)
                {
                    //  trim each field
                    var propertyName = field.Trim();

                    //  use reflection to get the property on the source object
                    //  need to include public and instance  because speciying a binding flag overwrites the already existing binding flags
                    var propertyInfo = typeof(TSource).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                    if (propertyInfo == null)
                    {
                        throw new Exception($"Property {propertyName} wasnt found on {typeof(TSource)}");
                    }

                    //  add propertyInnfo to list
                    propertyInfoList.Add(propertyInfo);

                }
            }

            //  run through the source objects
            foreach (TSource sourceObject in source)
            {
                //  create an ExpandoObject that will hold the selected propertyies and values
                var dataShapedObject = new ExpandoObject();

                //  get the value of each property we have to return
                //  for that we run through the list
                foreach (var propertyInfo in propertyInfoList)
                {
                    //  GetValue returns the value of the rpoperty on the source object
                    var propertyValue = propertyInfo.GetValue(sourceObject);

                    //  add the field to teh new ExpandoObject
                    ((IDictionary<string, object>)dataShapedObject).Add(propertyInfo.Name, propertyValue);
                }

                //  add the ExpandoObject to the list
                expandoObjectList.Add(dataShapedObject);
            }

            //  return the list
            return expandoObjectList;
        }
    }
}
