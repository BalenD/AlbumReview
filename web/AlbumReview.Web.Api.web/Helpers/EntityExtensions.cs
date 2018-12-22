using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;
using AlbumReview.Web.DtoModels;

namespace AlbumReview.Web.Helpers
{
    public static class EntityExtensions
    {
        public static ExpandoObject ShapeData(this ArtistDto artist, string fields)
        {
            return Datashaping(artist, fields);
        }

        public static ExpandoObject ShapeData(this AlbumDto album, string fields)
        {
            return Datashaping(album, fields);
        }

        public static ExpandoObject ShapeData(this ReviewDto review, string fields)
        {
            return Datashaping(review, fields);
        }

        private static ExpandoObject Datashaping<T>(this T source, string fields)
        {
            if (source == null)
            {
                throw new ArgumentNullException("Source cant be null");
            }

            var propertyInfoList = new List<PropertyInfo>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                var propertyInfos = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                propertyInfoList.AddRange(propertyInfos);
            }
            else
            {
                var fieldsAfterSplit = fields.Split(",");

                foreach (var field in fieldsAfterSplit)
                {
                    var propertyName = field.Trim();

                    var propertyInfo = typeof(T).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                    if (propertyInfo == null)
                    {
                        throw new Exception($"Property {propertyName} wasnt found on {typeof(T)}");
                    }


                    propertyInfoList.Add(propertyInfo);

                }
            }


            var dataShapedObject = new ExpandoObject();

            //  get the value of each property we have to return
            //  for that we run through the list
            foreach (var propertyInfo in propertyInfoList)
            {
                //  GetValue returns the value of the rpoperty on the source object
                var propertyValue = propertyInfo.GetValue(source);

                //  add the field to teh new ExpandoObject
                ((IDictionary<string, object>)dataShapedObject).Add(propertyInfo.Name, propertyValue);
            }

            return dataShapedObject;
        }
    }
}
