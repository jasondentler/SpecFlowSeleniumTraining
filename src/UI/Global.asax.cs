using System.Web.Mvc;
using System.Web.Routing;

namespace UI
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "WidgetsCreate",
                url: "widgets/create",
                defaults: new { controller = "Widgets", action = "Create" });

            routes.MapRoute(
                name: "WidgetsIndex",
                url: "widgets/{id}",
                defaults: new { controller = "Widgets", action = "Index" });

            routes.MapRoute(
                name: "WidgetsList",
                url: "widgets",
                defaults: new { controller = "Widgets", action = "List" });

            routes.MapRoute(
                "Default",
                "{*parameters}",
                new {controller = "Home", action = "Index", parameters = UrlParameter.Optional, id = UrlParameter.Optional});


        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
        }
    }
}