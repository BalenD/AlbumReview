using Microsoft.AspNetCore.Mvc.ActionConstraints;
using System;

namespace AlbumReview.Web.Api.Api.Helpers
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    public class RequestMatchesMediaTypeHeaderAttribute : Attribute, IActionConstraint
    {
        private readonly string[] _mediatypes;
        private readonly string _requestHeaderToMatch;
        public int Order
        {
            get
            {
                return 0;
            }
        }

        public RequestMatchesMediaTypeHeaderAttribute(string requestHeaderToMatch, string [] mediatypes)
        {
            _requestHeaderToMatch = requestHeaderToMatch;
            _mediatypes = mediatypes;
        }

        public bool Accept(ActionConstraintContext context)
        {
            var requestHeaders = context.RouteContext.HttpContext.Request.Headers;
            if (!requestHeaders.ContainsKey(_requestHeaderToMatch))
            {
                return false;
            }

            foreach (var mediaType in _mediatypes)
            {
                var mediaTypeMatches = string.Equals(requestHeaders[_requestHeaderToMatch].ToString(), mediaType, StringComparison.OrdinalIgnoreCase);
                if (mediaTypeMatches)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
