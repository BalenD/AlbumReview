namespace AlbumReview.Services.Web
{
    public interface ITypeHelperService
    {
        bool TypeHasProperties<T>(string fields);
    }
}
