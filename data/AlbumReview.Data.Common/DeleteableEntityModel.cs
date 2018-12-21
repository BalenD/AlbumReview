using System;

namespace AlbumReview.Data.Common
{
    public class DeleteableEntityModel<TKey> : BaseEntityModel<TKey>, IDeleteableEntity
    {
        public bool IsDeleted { get; set; }
        public DateTimeOffset? DeletedOn { get; set; }
    }
}
