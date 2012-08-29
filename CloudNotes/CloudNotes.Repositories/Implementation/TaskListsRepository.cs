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
            return _unitOfWork.Load<TaskListEntity>("TaskLists").ToList().Select(t => t.MapToTaskList()).AsQueryable();
        }

        public TaskList Get(string partitionKey, string rowKey)
        {
            var result = _unitOfWork.Get<TaskListEntity>("TaskLists", t => t.PartitionKey == partitionKey && t.RowKey == rowKey);
            return result != null ? result.MapToTaskList() : null;
        }

        public TaskList Get(Expression<Func<TaskList, bool>> filter)
        {
            return _unitOfWork.Get("TaskLists", filter);
        }

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

        public void Create(TaskList entityToCreate)
        {
            entityToCreate.PartitionKey = entityToCreate.Owner.RowKey;
            entityToCreate.RowKey = ShortGuid.NewGuid();

            var taskListEntity = entityToCreate.MapToTaskListEntity();
            _unitOfWork.Create(taskListEntity, "TaskLists");

            var sharePartitionKey = string.Format("{0}+{1}", entityToCreate.PartitionKey, entityToCreate.RowKey);
            var taskListShare = new TaskListShareEntity(sharePartitionKey, entityToCreate.Owner.RowKey);
            _unitOfWork.Create(taskListShare, "TaskListShares");
        }

        public void Update(TaskList entityToUpdate)
        {
            _unitOfWork.Update("TaskLists", entityToUpdate.MapToTaskListEntity());
        }

        public void Delete(TaskList entityToDelete)
        {
            _unitOfWork.Delete<TaskListEntity>("TaskLists", entityToDelete.PartitionKey, entityToDelete.RowKey);

            var sharePartitionKey = string.Format("{0}+{1}", entityToDelete.PartitionKey, entityToDelete.RowKey);
            var taskListShares = _unitOfWork.Load<TaskListShareEntity>("TaskListShares", ts => ts.PartitionKey == sharePartitionKey);

            foreach (var taskListShare in taskListShares)
            {
                _unitOfWork.Delete<TaskListShareEntity>("TaskListShares", taskListShare.PartitionKey, taskListShare.RowKey);
            }

            var taskListNotePartitionKey = sharePartitionKey;
            var taskListNotes = _unitOfWork.Load<TaskListNoteEntity>("TaskListNotes", ts => ts.PartitionKey == taskListNotePartitionKey);

            foreach (var taskListNoteEntity in taskListNotes)
            {
                _unitOfWork.Delete<TaskListNoteEntity>("TaskListNotes", taskListNoteEntity.PartitionKey, taskListNoteEntity.RowKey);
            }
        }

        public void AddShare(TaskList taskList, string userId)
        {
            var sharePartitionKey = string.Format("{0}+{1}", taskList.PartitionKey, taskList.RowKey);
            var share = new TaskListShareEntity(sharePartitionKey, userId);
            _unitOfWork.Create(share, "TaskListShares");
        }

        public void RemoveShare(TaskList taskList, string userId)
        {
            var sharePartitionKey = string.Format("{0}+{1}", taskList.PartitionKey, taskList.RowKey);
            _unitOfWork.Delete<TaskListShareEntity>("TaskListShares", sharePartitionKey, userId);
        }

        public bool HasPermissionToEdit(User user, TaskList taskList)
        {
            var taskListSharePartitionKey = string.Format("{0}+{1}", taskList.PartitionKey, taskList.RowKey);
            return _unitOfWork.Get<TaskListShareEntity>("TaskListShares", ts => ts.PartitionKey == taskListSharePartitionKey && ts.RowKey == user.RowKey) != null;
        }

        #endregion Public methods
    }
}