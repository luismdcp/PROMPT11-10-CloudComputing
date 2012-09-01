using System.Collections.Generic;
using System.Linq;
using CloudNotes.Domain.Services.Contracts;
using CloudNotes.Domain.Services.Models.WebAPI;
using CloudNotes.Domain.Services.Models.WebAPI.Extensions;

namespace CloudNotes.Domain.Services.Implementation
{
    /// <summary>
    /// Service to be used by the Web API for Tasks operations.
    /// </summary>
    public class TasksRestService : ITasksRestService
    {
        #region Fields

        private readonly INotesService _notesService;
        private readonly ITaskListsService _taskListsService;

        #endregion Fields

        #region Constructors

        public TasksRestService(INotesService notesService, ITaskListsService taskListsService)
        {
            _notesService = notesService;
            _taskListsService = taskListsService;
        }

        #endregion Constructors

        #region Public methods

        /// <summary>
        /// Get all the Tasks contained in a List.
        /// </summary>
        /// <param name="userId">Containing List partition key</param>
        /// <param name="resourceId">Containing List row key</param>
        /// <returns>All the Tasks in the List</returns>
        public IEnumerable<Task> GetAll(string userId, string resourceId)
        {
            var taskList = _taskListsService.Get(userId, resourceId);
            _notesService.LoadNotes(taskList);

            return taskList.Notes.Select(t => t.MapToTask());
        }

        /// <summary>
        /// Move a Task from his containing List to another List.
        /// </summary>
        /// <param name="task">Task to be moved</param>
        /// <param name="destinationList">List to be the moved Task container</param>
        /// <returns>The new row key from the moved Task</returns>
        public string Move(Task task, List destinationList)
        {
            return _notesService.MoveNote(task.MapToNote(), destinationList.MapToTaskList());
        }

        /// <summary>
        /// Get a Task by his partiton key and row key.
        /// </summary>
        /// <param name="userId">Task partition key</param>
        /// <param name="resourceId">Task row key</param>
        /// <returns>The Task if exists, null otherwise</returns>
        public Task Get(string userId, string resourceId)
        {
            var list = _notesService.Get(t => t.PartitionKey == userId && t.RowKey == resourceId);
            return list != null ? list.MapToTask() : null;
        }

        #endregion Public methods
    }
}