﻿using CloudNotes.Domain.Services.Contracts;
using CloudNotes.Domain.Services.Implementation;
using CloudNotes.Repositories.Contracts;
using CloudNotes.Repositories.Implementation;
using StructureMap;

namespace CloudNotes.Infrastructure.DependencyInjection
{
    public static class IoC
    {
        public static IContainer Initialize()
        {
            ObjectFactory.Initialize(x =>
            {
                x.Scan(scan =>
                {
                    scan.TheCallingAssembly();
                    scan.WithDefaultConventions();
                });

                x.For<IUnitOfWork>().HttpContextScoped().Use<AzureTablesUnitOfWork>();
                x.For<INotesRepository>().HttpContextScoped().Use<NotesRepository>();
                x.For<ITaskListsRepository>().HttpContextScoped().Use<TaskListsRepository>();
                x.For<IUsersRepository>().HttpContextScoped().Use<UsersRepository>();
                x.For<IFilesRepository>().HttpContextScoped().Use<FilesRepository>();
                x.For<INotesService>().HttpContextScoped().Use<NotesService>();
                x.For<ITaskListsService>().HttpContextScoped().Use<TaskListsService>();
                x.For<IUsersService>().HttpContextScoped().Use<UsersService>();
                x.For<IFilesService>().HttpContextScoped().Use<FilesService>();
                x.For<IListsRestService>().HttpContextScoped().Use<ListsRestService>();
                x.For<ITasksRestService>().HttpContextScoped().Use<TasksRestService>();
                x.For<IOAuthService>().HttpContextScoped().Use<OAuthService>();
                x.For<IOAuthRepository>().HttpContextScoped().Use<OAuthRepository>();
            });

            return ObjectFactory.Container;
        }
    }
}