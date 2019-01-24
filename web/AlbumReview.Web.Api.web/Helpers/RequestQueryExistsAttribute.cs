using Microsoft.AspNetCore.Mvc.ActionConstraints;
using System;

namespace AlbumReview.Web.Api.Api.Helpers
{
    [AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = true)]
    public class RequestQueryExistsAttribute : Attribute, IActionConstraint
    {
        public string[] QueryStringsToCheck { get; set; }
        public RequestQueryExistsAttribute(string[] querystringsTocheck)
        {
            QueryStringsToCheck = querystringsTocheck;
        }
        public int Order
        {
            get
            {
                return 0;
            }
        }

        public bool Accept(ActionConstraintContext context)
        {
            var queryStrings = context.RouteContext.HttpContext.Request.Query;
            foreach (var querystring in QueryStringsToCheck)
            {
                if (queryStrings.ContainsKey(querystring))
                {
                    return true;
                }

            }
            return false;
        }
    }
}
