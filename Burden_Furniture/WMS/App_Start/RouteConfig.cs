using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WMS
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
            name: "Unauthorized",
            url: "unauthorized/{action}/{id}",
            defaults: new { controller = "Error", action = "Unauthorized", id = UrlParameter.Optional }
        );
            routes.MapRoute(
             name: "Logout",
             url: "logout/{action}/{id}",
             defaults: new { controller = "Account", action = "Logout", id = UrlParameter.Optional }
         );
            routes.MapRoute(
             name: "Login",
             url: "login/{action}/{id}",
             defaults: new { controller = "Account", action = "Login", id = UrlParameter.Optional }
         );
            routes.MapRoute(name: "PhieuNhapHang", url: "phieu-nhap-hang", defaults: new { controller = "PhieuNhapHang", action = "Index", id = UrlParameter.Optional });
            routes.MapRoute(name: "PhieuNhapHangChiTiet", url: "phieu-nhap-hang/phieu-nhap", defaults: new { controller = "PhieuNhapHang", action = "PhieuNhap", id = UrlParameter.Optional });
            routes.MapRoute(name: "InQRCode1", url: "qr-code-1", defaults: new { controller = "PurchaseOrder", action = "InQRCode1", id = UrlParameter.Optional });
            routes.MapRoute(name: "ShowInQRCode1", url: "qr-code-1-in", defaults: new { controller = "PurchaseOrder", action = "ShowInQRCode1", id = UrlParameter.Optional });
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
