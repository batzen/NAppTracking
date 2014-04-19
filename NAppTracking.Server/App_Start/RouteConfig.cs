namespace NAppTracking.Server
{
    using System.Web.Mvc;
    using System.Web.Routing;

    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "ExceptionReportIndex",
                url: "Application/{applicationId}/ExceptionReport/{page}",
                defaults: new { controller = "ExceptionReport", action = "Index", page = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "ExceptionReportFile",
                url: "Application/{applicationId}/ExceptionReport/File/{storageId}",
                defaults: new { controller = "ExceptionReport", action = "File" }
            );

            routes.MapRoute(
                name: "ExceptionReport",
                url: "Application/{applicationId}/ExceptionReport/{action}/{exceptionReportId}",
                defaults: new { controller = "ExceptionReport", action = "Index", exceptionReportId = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}