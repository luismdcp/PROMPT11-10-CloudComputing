using CloudNotes.Domain.Services.Contracts;
using CloudNotes.Domain.Services.Implementation;
using CloudNotes.Repositories.Contracts;
using CloudNotes.Repositories.Implementation;
using StructureMap;

namespace CloudNotes.Infrastructure.DependencyInjection
{
    public static class Bootstrapper
    {
        public static void BootstrapStructureMap()
        {
            ObjectFactory.Initialize(x =>
            {
                x.For<IUnitOfWork>().Use<AzureTablesUnitOfWork>();
                x.For<INotesRepository>().Use<NotesRepository>();
                x.For<ITaskListsRepository>().Use<TaskListsRepository>();
                x.For<IUsersRepository>().Use<UsersRepository>();
                x.For<INotesService>().Use<NotesService>();
                x.For<ITaskListsService>().Use<TaskListsService>();
                x.For<IUsersService>().Use<UsersService>();
            });
        }
    }
}