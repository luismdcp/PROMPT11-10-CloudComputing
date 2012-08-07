using System.Security.Principal;
using System.Web.Mvc;
using System.Web.Routing;
using CloudNotes.Infrastructure.DependencyInjection;
using CloudNotes.Repositories;
using CloudNotes.WebRole.Helpers;

namespace CloudNotes.WebRole
{
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
                "Notes", // Route name
                "TaskList/{taskListTitle}/Notes/{action}/{noteTitle}", // URL with parameters
                new { controller = "Notes", action = "Index", noteTitle = UrlParameter.Optional } // Parameter defaults
            );

            routes.MapRoute(
                "UsersAssociateTaskList", // Route name
                "Users/Associate/TaskList/{taskListTitle}", // URL with parameters
                new { controller = "Users", action = "AssociateUsersToTaskList" } // Parameter defaults
            );

            routes.MapRoute(
                "UsersAssociateNote", // Route name
                "Users/Associate/TaskList/{taskListTitle}/Note/{noteTitle}", // URL with parameters
                new { controller = "Users", action = "AssociateUsersToNote" } // Parameter defaults
            );

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{taskListTitle}", // URL with parameters
                new { controller = "Users", action = "Home", taskListTitle = UrlParameter.Optional } // Parameter defaults
            );

        }

        protected void Application_Start()
        {
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());

            StructureMapBootstrapper.Start();

            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
            ModelBinders.Binders[typeof(IPrincipal)] = new PrincipalModelBinder();
            TablesBuilder.InitializeTables();
        }
    }
}