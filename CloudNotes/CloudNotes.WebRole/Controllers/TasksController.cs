using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using CloudNotes.Domain.Services.Contracts;
using CloudNotes.Domain.Services.Models.WebAPI;
using StructureMap;

namespace CloudNotes.WebRole.Controllers
{
    public class TasksController : ApiController
    {
        #region Fields

        private readonly ITasksRestService _tasksService;
        private readonly IListsRestService _listsService;

        #endregion Fields

        #region Constructors

        public TasksController()
        {
            _tasksService = ObjectFactory.GetInstance<ITasksRestService>();
            _listsService = ObjectFactory.GetInstance<IListsRestService>();
        }

        #endregion Constructors

        #region Actions

        [HttpGet]
        public IEnumerable<Task> All(string userId, string resourceId)
        {
            var tasks = _tasksService.GetAll(userId, resourceId).ToList();

            for (int i = 0; i < tasks.Count; i++)
            {
                var uri = Url.Route("WebAPI", new { controller = "Tasks", action = "Details", userId = tasks[i].PartitionKey, resourceId = tasks[i].RowKey });
                var absoluteUrl = new Uri(Request.RequestUri, uri).AbsoluteUri;

                var selfLink = new Link
                                {
                                    Name = "self",
                                    Rel = "http://api.relations.wrml.org/common/self",
                                    Href = absoluteUrl
                                };

                tasks[i].Links.Add(selfLink);
            }

            return tasks;
        }

        [HttpGet]
        public Task Details(string userId, string resourceId)
        {
            var task = _tasksService.Get(userId, resourceId);

            if (task == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            var uri = Url.Route("WebAPI", new { controller = "Tasks", action = "Details", userId, resourceId });
            var absoluteUrl = new Uri(Request.RequestUri, uri).AbsoluteUri;

            var selfLink = new Link
                            {
                                Name = "self",
                                Rel = "http://api.relations.wrml.org/common/self",
                                Href = absoluteUrl
                            };

            uri = Url.Route("WebAPI", new { controller = "Tasks", action = "All", userId });
            absoluteUrl = new Uri(Request.RequestUri, uri).AbsoluteUri;

            var allLink = new Link
                                {
                                    Name = "self",
                                    Rel = "http://api.relations.wrml.org/common/all",
                                    Href = absoluteUrl
                                };

            task.Links.Add(selfLink);
            task.Links.Add(allLink);

            return task;
        }

        [HttpPost]
        public Task Move(string userId, string resourceId, [FromBody] string destinationListPartitionKey, [FromBody] string destinationListRowKey)
        {
            var task = _tasksService.Get(userId, resourceId);

            if (task == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            var list = _listsService.Get(destinationListPartitionKey, destinationListRowKey);

            if (list == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            var movedTaskRowKey = _tasksService.Move(task, list);
            var movedTask = new Task(task.PartitionKey, movedTaskRowKey, task.Title, task.Content, task.IsClosed);

            var uri = Url.Route("WebAPI", new { controller = "Tasks", action = "Details", userId, movedTask.RowKey });
            var absoluteUrl = new Uri(Request.RequestUri, uri).AbsoluteUri;

            var selfLink = new Link
                                {
                                    Name = "self",
                                    Rel = "http://api.relations.wrml.org/common/self",
                                    Href = absoluteUrl
                                };

            uri = Url.Route("WebAPI", new { controller = "Tasks", action = "All", userId });
            absoluteUrl = new Uri(Request.RequestUri, uri).AbsoluteUri;

            var allLink = new Link
                        {
                            Name = "self",
                            Rel = "http://api.relations.wrml.org/common/all",
                            Href = absoluteUrl
                        };

            uri = Url.Route("WebAPI", new { controller = "Lists", action = "Details", list.PartitionKey, list.RowKey });
            absoluteUrl = new Uri(Request.RequestUri, uri).AbsoluteUri;

            var parentLink = new Link
                            {
                                Name = "self",
                                Rel = "http://api.relations.wrml.org/common/parent",
                                Href = absoluteUrl
                            };

            movedTask.Links.Add(selfLink);
            movedTask.Links.Add(allLink);
            movedTask.Links.Add(parentLink);

            return movedTask;
        }

        #endregion Actions
    }
}