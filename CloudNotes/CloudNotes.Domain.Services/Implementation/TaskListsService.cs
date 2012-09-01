using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using CloudNotes.Domain.Entities;
using CloudNotes.Domain.Services.Contracts;
using CloudNotes.Repositories.Contracts;

namespace CloudNotes.Domain.Services.Implementation
{

    /// <summary>
    /// Service class to manage all the operations related to TaskLists.
    /// </summary>
    public class TaskListsService : ITaskListsService
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly ITaskListsRepository _taskListsRepository;
        private readonly INotesRepository _notesRepository;

        #endregion Fields

        #region Constructors

        public TaskListsService(IUnitOfWork unitOfWork, ITaskListsRepository taskListsRepository, INotesRepository notesRepository)
        {
            _unitOfWork = unitOfWork;
            _taskListsRepository = taskListsRepository;
            _notesRepository = notesRepository;
        }

        #endregion Constructors

        #region Public methods


        /// <summary>
        /// Load all TaskLists.
        /// </summary>
        /// <returns>IQueryable of all TaskLists</returns>
        public IQueryable<TaskList> Load()
        {
            return _taskListsRepository.Load();
        }

        /// <summary>
        /// Get a TaskList by his partiton key and row key.
        /// </summary>
        /// <param name="partitionKey">The TaskList partition key</param>
        /// <param name="rowKey">The TaskList row key</param>
        /// <returns>A TaskList if exists, null otherwise</returns>
        public TaskList Get(string partitionKey, string rowKey)
        {
            return _taskListsRepository.Get(partitionKey, rowKey);
        }

        /// <summary>
        /// Get a TaskList by a filter.
        /// </summary>
        /// <param name="filter">Lambda expression filter</param>
        /// <returns>A TaskList if exists, null otherwise</returns>
        public TaskList Get(Expression<Func<TaskList, bool>> filter)
        {
            return _taskListsRepository.Get(filter);
        }

        /// <summary>
        /// Get a TaskList by his combined partition and row keys.
        /// </summary>
        /// <param name="combinedKeys">The User partition and row keys combined</param>
        /// <returns>A TaskList if exists, null otherwise</returns>
        public TaskList Get(string combinedKeys)
        {
            var taskListKeys = combinedKeys.Split('+');
            var taskListPartitionKey = taskListKeys[0];
            var taskListRowKey = taskListKeys[1];
            return Get(taskListPartitionKey, taskListRowKey);
        }

        /// <summary>
        /// Create a TaskList.
        /// </summary>
        /// <param name="entityToCreate">TaskList to create</param>
        public void Create(TaskList entityToCreate)
        {
            _taskListsRepository.Create(entityToCreate);
            _unitOfWork.SubmitChanges();
        }

        /// <summary>
        /// Update a TaskList.
        /// </summary>
        /// <param name="entityToUpdate">TaskList to update</param>
        public void Update(TaskList entityToUpdate)
        {
            _taskListsRepository.Update(entityToUpdate);
            _unitOfWork.SubmitChanges();
        }

        /// <summary>
        /// Delete a TaskList and his Notes.
        /// </summary>
        /// <param name="entityToDelete">TaskList to delete</param>
        public void Delete(TaskList entityToDelete)
        {
            _notesRepository.LoadNotes(entityToDelete);

            foreach (var note in entityToDelete.Notes)
            {
                _notesRepository.Delete(note);
            }

            _taskListsRepository.Delete(entityToDelete);

            _unitOfWork.SubmitChanges();
        }

        /// <summary>
        /// Get all the TaskList that were shared to the User.
        /// </summary>
        /// <param name="user">User to find the TaskLists that were shared with him</param>
        /// <returns>All taskLists that were shared to the User</returns>
        public IEnumerable<TaskList> GetShared(User user)
        {
            return _taskListsRepository.GetShared(user);
        }

        /// <summary>
        /// Load the containing TaskList for a Note.
        /// </summary>
        /// <param name="note">Note to load the containing TaskList</param>
        public void LoadContainer(Note note)
        {
            _taskListsRepository.LoadContainer(note);
        }

        /// <summary>
        /// Add a User to the TaskList share list.
        /// </summary>
        /// <param name="taskList">TaskList to share with the User</param>
        /// <param name="userId">Row key of the User to share the TaskList</param>
        public void AddShare(TaskList taskList, string userId)
        {
            _taskListsRepository.AddShare(taskList, userId);
            _unitOfWork.SubmitChanges();
        }

        /// <summary>
        /// Remove a User from the TaskList share list.
        /// </summary>
        /// <param name="taskList">TaskList which is shared with the User</param>
        /// <param name="userId">Row key of the User to remove from the TaskList share list</param>
        public void RemoveShare(TaskList taskList, string userId)
        {
            _taskListsRepository.RemoveShare(taskList, userId);
            _unitOfWork.SubmitChanges();
        }

        /// <summary>
        /// Check if the User has permission to edit the TaskList.
        /// </summary>
        /// <param name="user">User to check the permission</param>
        /// <param name="taskList">TaskList to be edited</param>
        /// <returns>True if the User has permission to edit the TaskList, False otherwise</returns>
        public bool HasPermissionToEdit(User user, TaskList taskList)
        {
            return _taskListsRepository.HasPermissionToEdit(user, taskList);
        }

        #endregion Public methods
    }
}