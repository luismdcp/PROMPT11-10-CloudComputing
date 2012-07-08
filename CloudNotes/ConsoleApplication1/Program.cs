
using CloudNotes.Repositories.Implementation;
using DependencyInjection;
using StructureMap;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            Bootstrapper.BootstrapStructureMap();
            var t = ObjectFactory.GetInstance<NotesRepository>();
        }
    }
}
