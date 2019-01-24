using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;

namespace AlbumReview.Web.Api.Helpers
{
    public class ErrorProcessingEntityObjectResult : ObjectResult
    {
        public ErrorProcessingEntityObjectResult(ModelStateDictionary value) : base(new SerializableError(value))
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            StatusCode = 422;
        }
    }
}
