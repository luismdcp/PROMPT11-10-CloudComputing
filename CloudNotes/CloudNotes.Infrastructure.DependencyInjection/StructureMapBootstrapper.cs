using System.Web.Mvc;

namespace CloudNotes.Infrastructure.DependencyInjection
{
    public static class StructureMapBootstrapper
    {
        public static void Start()
        {
            var container = IoC.Initialize();
            DependencyResolver.SetResolver(new StructureMapDependencyResolver(container));
        }
    }
}