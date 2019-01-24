namespace AlbumReview.Web.Api.DtoModels
{
    public class LinkedEntityWithPaginationMetadataDto<T>
    {
        public PaginationMetadata Metadata { get; set; }
        public T Records { get; set; }
        public LinkedEntityWithPaginationMetadataDto(PaginationMetadata metadata, T records)
        {
            Metadata = metadata;
            Records = records;
        }
    }
}
