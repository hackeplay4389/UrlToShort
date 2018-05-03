using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace UrlToShort
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "admin",
                url: "admin",
                defaults: new { controller = "Main", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
               name: "manager",
               url: "manager",
               defaults: new { controller = "Main", action = "Manager", id = UrlParameter.Optional }
           );

            routes.MapRoute(
             name: "Home",
             url: "{id}",
             defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
          );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}