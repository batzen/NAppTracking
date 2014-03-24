namespace NAppTracking.Server
{
    using System.Web.Mvc;
    using NAppTracking.Server.Filters;

    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
