using System.Web.Mvc;

namespace ASPNETWebAppMVCStudentApp.Filters
{
    public class SessionAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(
            System.Web.HttpContextBase httpContext)
        {
            return httpContext.Session["Username"] != null;
        }

        protected override void HandleUnauthorizedRequest(
            AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(
                new System.Web.Routing.RouteValueDictionary
                {
                    { "controller", "Account" },
                    { "action", "Login" }
                });
        }
    }
}
