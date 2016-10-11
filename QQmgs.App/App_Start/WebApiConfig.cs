using Microsoft.Owin.Security.OAuth;
using System.Web.Http;

namespace Twitter.App
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            // Configure Web API to use only Bearer token authentication.
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                "DefaultApi",
                "api/{controller}/{id}",
                new {id = RouteParameter.Optional});

            // Enforce HTTPS
            // TODO: enable global HTTPS
            // config.Filters.Add(new Filters.RequireHttpsAttribute());
        }
    }
}