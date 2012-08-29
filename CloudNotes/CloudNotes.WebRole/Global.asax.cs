using System.Security.Principal;
using System.Web.Mvc;
using System.Web.Routing;
using CloudNotes.Infrastructure.DependencyInjection;
using CloudNotes.Repositories;
using CloudNotes.WebRole.Helpers;
using System.Web.Http;

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

            routes.MapHttpRoute("Default API Route", "api/{controller}/{id}", new { controller = "Lists", id = UrlParameter.Optional });

            routes.MapRoute(
                "Files", // Route name
                "Files/Note/{noteOwnerId}/{noteId}/{action}/{fileName}", // URL with parameters
                new { controller = "Files", action = "Index", fileName = UrlParameter.Optional } // Parameter defaults
            );

            routes.MapRoute(
                "NotesIndexCreate", // Route name
                "Notes/TaskList/{taskListOwnerId}/{taskListId}/{action}", // URL with parameters
                new { controller = "Notes", action = "Index" } // Parameter defaults
            );

            routes.MapRoute(
                "Notes", // Route name
                "Notes/{noteOwnerId}/{noteId}/{action}",
                new { controller = "Notes" } // Parameter defaults
            );

            routes.MapRoute(
                "TaskLists", // Route name
                "TaskLists/{taskListOwnerId}/{taskListId}/{action}",
                new { controller = "TaskLists", taskListOwnerId = UrlParameter.Optional, taskListId = UrlParameter.Optional } // Parameter defaults
            );

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}", // URL with parameters
                new { controller = "Users", action = "Home" } // Parameter defaults
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
            ContainerBuilder.InitializeContainer();
        }
    }
}