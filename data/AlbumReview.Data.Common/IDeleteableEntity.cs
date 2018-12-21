using System;

namespace AlbumReview.Data.Common
{
    public interface IDeleteableEntity
    {
        bool IsDeleted { get; set; }
        DateTimeOffset? DeletedOn { get; set; }
    }
}
