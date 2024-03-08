using System.Web.Mvc;
using System.Web.WebSockets;

namespace STech_Web.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "OtherSettings",
                "admin/other",
                new { controller = "OtherSettings", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
                "ProductDetail",
                "admin/product/{id}",
                new { controller = "Products", action = "Detail", id = UrlParameter.Optional }
            );

            context.MapRoute(
                "Admin_home",
                "admin/",
                new { controller = "Dashboard", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
                "Admin_default",
                "Admin/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}