using System;

namespace AlbumReview.Data.Common
{
    public interface IAuditInfo
    {
        DateTimeOffset CreatedOn { get; set; }
        DateTimeOffset? ModifiedOn { get; set; }
    }
}
