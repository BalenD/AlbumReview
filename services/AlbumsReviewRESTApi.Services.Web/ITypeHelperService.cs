namespace AlbumsReviewRESTApi.Services.Web
{
    public interface ITypeHelperService
    {
        bool TypeHasProperties<T>(string fields);
    }
}
