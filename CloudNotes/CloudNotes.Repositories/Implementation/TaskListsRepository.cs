using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using CloudNotes.Domain.Entities;
using CloudNotes.Repositories.Contracts;
using CloudNotes.Repositories.Entities;
using CloudNotes.Repositories.Entities.Relation;
using CloudNotes.Repositories.Extensions;
using CloudNotes.Repositories.Helpers;

namespace CloudNotes.Repositories.Implementation
{
    /// <summary>
    /// Repository to manage all the actions related to the TasksLists, TaskListNotes and TaskListShares Azure Tables.
    /// </summary>
    public class TaskListsRepository : ITaskListsRepository
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;

        #endregion Fields

        #region Constructors

        public TaskListsRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Constructors

        #region Public methods

        /// <summary>
        /// Loads all the entities in the TaskLists Azure Table.
        /// </summary>
        /// <returns>IQueryable of all the TaskList entities</returns>
        public IQueryable<TaskList> Load()
        {
            return _unitOfWork.Load<TaskListEntity>("TaskLists").ToList().Select(t => t.MapToTaskList()).AsQueryable();
        }

        /// <summary>
        /// Gets a single entity from the TaskLists Azure Table and maps it to a TaskList domain object.
        /// </summary>
        /// <param name="partitionKey">The TaskList partitionkey</param>
        /// <param name="rowKey">The TaskList rowkey</param>
        /// <returns>A TaskList if exists, null otherwise</returns>
        public TaskList Get(string partitionKey, string rowKey)
        {
            var result = _unitOfWork.Get<TaskListEntity>("TaskLists", t => t.PartitionKey == partitionKey && t.RowKey == rowKey);
            return result != null ? result.MapToTaskList() : null;
        }

        /// <summary>
        /// Gets a single entity from the TaskLists Azure Table by filter and maps it to a TaskList domain object.
        /// </summary>
        /// <param name="filter">Lambda to filter the TaskList</param>
        /// <returns>A TaskList if exists, null otherwise</returns>
        public TaskList Get(Expression<Func<TaskList, bool>> filter)
        {
            return _unitOfWork.Get("TaskLists", filter);
        }

        /// <summary>
        /// Gets all the TaskLists that were shared to or created by the User.
        /// </summary>
        /// <param name="user">The User to know his Shares</param>
        /// <returns>List of all the TaskLists shared to or created by User</returns>
        public IEnumerable<TaskList> GetShared(User user)
        {
            var taskLists = new List<TaskList>();
            var taskListShares = _unitOfWork.Load<TaskListShareEntity>("TaskListShares", ts => ts.RowKey == user.RowKey);

            foreach (var taskListShare in taskListShares)
            {
                var keys = taskListShare.PartitionKey.Split('+');
                var taskListPartitionKey = keys[0];
                var taskListRowKey = keys[1];

                var taskList = _unitOfWork.Get<TaskListEntity>("TaskLists", t => t.PartitionKey == taskListPartitionKey && t.RowKey == taskListRowKey);

                if (taskList != null)
                {
                    taskLists.Add(taskList.MapToTaskList());
                }
            }

            return taskLists.AsEnumerable();
        }

        /// <summary>
        /// Gets and fills the Note parent container (TaskList).
        /// </summary>
        /// <param name="note">The Note to fill the parent container</param>
        public void LoadContainer(Note note)
        {
            var taskListNoteRowKey = string.Format("{0}+{1}", note.PartitionKey, note.RowKey);
            var taskListNote = _unitOfWork.Get<TaskListEntity>("TaskListNotes", tn => tn.RowKey == taskListNoteRowKey);

            var containerKeys = taskListNote.PartitionKey.Split('+');
            var containerPartitionKey = containerKeys[0];
            var containerRowKey = containerKeys[1];
            var container = Get(containerPartitionKey, containerRowKey);

            if (container != null)
            {
                note.Container = container;
            }
        }

        /// <summary>
        /// Creates a new TaskList.
        /// </summary>
        /// <param name="entityToCreate">TaskList domain object with the properties to map to an TaskLists table entity</param>
        public void Create(TaskList entityToCreate)
        {
            entityToCreate.PartitionKey = entityToCreate.Owner.RowKey;
            entityToCreate.RowKey = ShortGuid.NewGuid();

            var taskListEntity = entityToCreate.MapToTaskListEntity();
            _unitOfWork.Create(taskListEntity, "TaskLists");

            // the User that creates the new TaskList is automatically add in the TaskList Shares list.
            var sharePartitionKey = string.Format("{0}+{1}", entityToCreate.PartitionKey, entityToCreate.RowKey);
            var taskListShare = new TaskListShareEntity(sharePartitionKey, entityToCreate.Owner.RowKey);
            _unitOfWork.Create(taskListShare, "TaskListShares");
        }

        /// <summary>
        /// Updates an entity in the TaskLists Azure Table.
        /// </summary>
        /// <param name="entityToUpdate">TaskList domain object with the properties to update an existing TaskLists table entity</param>
        public void Update(TaskList entityToUpdate)
        {
            _unitOfWork.Update("TaskLists", entityToUpdate.MapToTaskListEntity());
        }

        /// <summary>
        /// Deletes an entity from the TaskLists Azure Table and all the entities in the related Azure Tables.
        /// </summary>
        /// <param name="entityToDelete">TaskList domain object with the properties to delete an existing TaskLists table entity</param>
        public void Delete(TaskList entityToDelete)
        {
            _unitOfWork.Delete<TaskListEntity>("TaskLists", entityToDelete.PartitionKey, entityToDelete.RowKey);

            // delete all the entities related to the TaskList Shares list.
            var sharePartitionKey = string.Format("{0}+{1}", entityToDelete.PartitionKey, entityToDelete.RowKey);
            var taskListShares = _unitOfWork.Load<TaskListShareEntity>("TaskListShares", ts => ts.PartitionKey == sharePartitionKey);

            foreach (var taskListShare in taskListShares)
            {
                _unitOfWork.Delete<TaskListShareEntity>("TaskListShares", taskListShare.PartitionKey, taskListShare.RowKey);
            }

            // delete all the TaskList Notes.
            var taskListNotePartitionKey = sharePartitionKey;
            var taskListNotes = _unitOfWork.Load<TaskListNoteEntity>("TaskListNotes", ts => ts.PartitionKey == taskListNotePartitionKey);

            foreach (var taskListNoteEntity in taskListNotes)
            {
                _unitOfWork.Delete<TaskListNoteEntity>("TaskListNotes", taskListNoteEntity.PartitionKey, taskListNoteEntity.RowKey);
            }
        }

        /// <summary>
        /// Share the TaskList with a User.
        /// </summary>
        /// <param name="taskList">The TaskList to be shared</param>
        /// <param name="userId">RowKey from the User</param>
        public void AddShare(TaskList taskList, string userId)
        {
            var sharePartitionKey = string.Format("{0}+{1}", taskList.PartitionKey, taskList.RowKey);
            var share = new TaskListShareEntity(sharePartitionKey, userId);
            _unitOfWork.Create(share, "TaskListShares");
        }

        /// <summary>
        /// Remove an existing Share from the TaskList.
        /// </summary>
        /// <param name="taskList">The TaskList with the Share to be removed</param>
        /// <param name="userId">RowKey from the User</param>
        public void RemoveShare(TaskList taskList, string userId)
        {
            var sharePartitionKey = string.Format("{0}+{1}", taskList.PartitionKey, taskList.RowKey);
            _unitOfWork.Delete<TaskListShareEntity>("TaskListShares", sharePartitionKey, userId);
        }

        /// <summary>
        /// Checks if a User has permissions to edit a TaskList, if it is in the TaskList Share list.
        /// </summary>
        /// <param name="user">The User to check the permission</param>
        /// <param name="taskList">The TaskList the User wants to edit</param>
        /// <returns>True if the User is in the TaskList Shares list, False otherwise</returns>
        public bool HasPermissionToEdit(User user, TaskList taskList)
        {
            var taskListSharePartitionKey = string.Format("{0}+{1}", taskList.PartitionKey, taskList.RowKey);
            return _unitOfWork.Get<TaskListShareEntity>("TaskListShares", ts => ts.PartitionKey == taskListSharePartitionKey && ts.RowKey == user.RowKey) != null;
        }

        #endregion Public methods
    }
}