using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace STech_Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
             name: "FilterPost",
             url: "collections/filtercollections",
             defaults: new { controller = "Collections", action = "FilterCollections" }
           );

            routes.MapRoute(
             name: "Filter",
             url: "collections/{id}",
             defaults: new { controller = "Collections", action = "GetProduct", id = UrlParameter.Optional }
           );

            routes.MapRoute(
                name: "Search",
                url: "search/{search}",
                defaults: new { controller = "Collections", action = "Search", search = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Detail",
                url: "product/{id}",
                defaults: new { controller = "Product", action = "Detail", id = UrlParameter.Optional }
            );

            routes.MapRoute(
               name: "About",
               url: "about",
               defaults: new { controller = "Home", action = "About"}
           );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "STech_Web.Controllers" }
            );
        }
    }
}
