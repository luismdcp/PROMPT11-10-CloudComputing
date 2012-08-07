using System;
using System.Collections.Generic;
using System.Linq;
using CloudNotes.Domain.Entities;
using CloudNotes.Repositories.Contracts;
using CloudNotes.Repositories.Entities;
using CloudNotes.Repositories.Entities.Relation;
using CloudNotes.Repositories.Extensions;

namespace CloudNotes.Repositories.Implementation
{
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

        public IQueryable<TaskList> Load()
        {
            return _unitOfWork.Load<TaskListTableEntry>("TaskLists").ToList().Select(t => t.MapToTaskList()).AsQueryable();
        }

        public TaskList Get(string partitionKey, string rowKey)
        {
            var result = _unitOfWork.Get<TaskListTableEntry>("TaskLists", partitionKey, rowKey);
            return result != null ? result.MapToTaskList() : null;
        }

        public TaskList GetByTitleAndOwner(string title, User owner)
        {
            var result = _unitOfWork.Load<TaskListTableEntry>("TaskLists").Where(t => t.PartitionKey == owner.RowKey && t.Title == title).FirstOrDefault();
            return result != null ? result.MapToTaskList() : null;
        }

        public IEnumerable<TaskList> GetTaskListsAssociatedByUser(User user)
        {
            var taskLists = new List<TaskList>();
            var taskListsAssociatedToUserEntries = _unitOfWork.Load<TaskListAssociatedUserTableEntry>("TaskListAssociatedUsers").Where(t => t.RowKey == user.RowKey);

            foreach (var taskListAssociatedUserTableEntry in taskListsAssociatedToUserEntries)
            {
                var taskList = _unitOfWork.Load<TaskListTableEntry>("TaskLists").Where(t => t.RowKey == taskListAssociatedUserTableEntry.PartitionKey).FirstOrDefault();

                if (taskList != null)
                {
                    taskLists.Add(taskList.MapToTaskList());
                }
            }

            return taskLists.AsEnumerable();
        }

        public IEnumerable<TaskList> GetTaskListsOwnedByUser(User user)
        {
            var taskLists = new List<TaskList>();
            var taskListsOwnedByUserEntries = _unitOfWork.Load<TaskListTableEntry>("TaskLists").Where(t => t.PartitionKey == user.RowKey);

            foreach (var taskListsOwnedByUserEntry in taskListsOwnedByUserEntries)
            {
                taskLists.Add(taskListsOwnedByUserEntry.MapToTaskList());
            }

            return taskLists.AsEnumerable();
        }

        public void Create(TaskList entityToCreate)
        {
            entityToCreate.PartitionKey = entityToCreate.Owner.RowKey;
            entityToCreate.RowKey = Guid.NewGuid().ToString();

            var taskListTableEntry = entityToCreate.MapToTaskListTableEntry();
            _unitOfWork.Create(taskListTableEntry, "TaskLists");

            var taskListAssociatedUsersTableEntry = new TaskListAssociatedUserTableEntry(entityToCreate.RowKey, entityToCreate.Owner.RowKey);
            _unitOfWork.Create(taskListAssociatedUsersTableEntry, "TaskListAssociatedUsers");
        }

        public void Update(TaskList entityToUpdate)
        {
            _unitOfWork.Update("TaskLists", entityToUpdate.MapToTaskListTableEntry());
        }

        public void Delete(TaskList entityToDelete)
        {
            _unitOfWork.Delete<TaskListTableEntry>("TaskLists", entityToDelete.PartitionKey, entityToDelete.RowKey);

            var associatedUsers = _unitOfWork.Load<TaskListAssociatedUserTableEntry>("TaskListAssociatedUsers").Where(tu => tu.PartitionKey == entityToDelete.RowKey);

            foreach (var taskListAssociatedUserTableEntry in associatedUsers)
            {
                _unitOfWork.Delete<TaskListAssociatedUserTableEntry>("TaskListAssociatedUsers", taskListAssociatedUserTableEntry.PartitionKey, taskListAssociatedUserTableEntry.RowKey);
            }
        }

        public void AddAssociatedUser(TaskList taskList, User userToAdd)
        {
            var taskListAssociatedUsersTableEntry = new TaskListAssociatedUserTableEntry(taskList.PartitionKey, userToAdd.RowKey);
            _unitOfWork.Create(taskListAssociatedUsersTableEntry, "TaskListAssociatedUsers");
        }

        public void RemoveAssociatedUser(TaskList taskList, User userToRemove)
        {
            _unitOfWork.Delete<TaskListAssociatedUserTableEntry>("TaskListAssociatedUsers", taskList.RowKey, userToRemove.RowKey);
        }

        #endregion Public methods
    }
}