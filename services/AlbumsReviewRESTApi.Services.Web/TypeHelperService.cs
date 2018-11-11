using System.Reflection;

namespace AlbumsReviewRESTApi.Services.Web
{
    public class TypeHelperService : ITypeHelperService
    {
        public bool TypeHasProperties<T>(string fields)
        {
            if (string.IsNullOrWhiteSpace(fields))
            {
                return true;
            }

            //  split the fields in the string by a comma
            var fieldsAfterSplit = fields.Split(",");

            //  check if the requested fields exist on soruce

            foreach (var field in fieldsAfterSplit)
            {

                //  trim the field
                var propertyName = field.Trim();

                //  use reflection to check if the property
                //  can be found on T
                var propertyInfo = typeof(T).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                //  it cant be found return false
                if (propertyInfo == null)
                {
                    return false;
                }
            }

            //  all checks out, return true
            return true;
        }
    }
}
