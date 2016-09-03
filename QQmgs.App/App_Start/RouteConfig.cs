using System.Web.Http;

namespace Twitter.App
{
    using System.Web.Mvc;
    using System.Web.Routing;

    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapMvcAttributeRoutes();

            routes.MapRoute(
                name: "Default", 
                url: "{controller}/{action}/{id}",
                defaults: new {controller = "Account", action = "Register", id = UrlParameter.Optional},
                namespaces: new[]
                {
                    "Twitter.App.ManageController"
                });
        }
    }
}