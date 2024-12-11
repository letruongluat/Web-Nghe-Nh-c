using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace PlayMusic
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
            routes.MapRoute(
               name: "Admin",
               url: "Admin/{action}/{id}",
               defaults: new { controller = "Admin", action = "Index", id = UrlParameter.Optional }
           );
            routes.MapRoute(
        name: "DeleteUser",
        url: "Admin/DeleteUser/{id}",
        defaults: new { controller = "Admin", action = "DeleteUser", id = UrlParameter.Optional },
        namespaces: new[] { "PlayMusic.Controllers" }
    ); routes.MapRoute(
     name: "RemoveFromFavorite",
     url: "Song/RemoveFromFavorite",
     defaults: new { controller = "Song", action = "RemoveFromFavorite" }
 );
            routes.MapRoute(
           name: "MusicSearch",
           url: "Music/Search",
           defaults: new { controller = "Music", action = "Search" }
       );


        }
    }
}
