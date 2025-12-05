using System;
using System.Web;
using System.Web.Mvc;

namespace QL_KhachSan.Filters
{
    public class AdminAuthorize : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            return httpContext.Session["Admin"] != null;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(
                new System.Web.Routing.RouteValueDictionary
                {
            {"area", "Admin"},
            {"controller", "Account"},
            {"action", "Login"}
                });
        }

    }
}
