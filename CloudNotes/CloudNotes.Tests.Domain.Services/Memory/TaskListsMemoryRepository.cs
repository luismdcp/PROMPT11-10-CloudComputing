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

namespace CloudNotes.Tests.Domain.Services.Memory
{
    public class TaskListsMemoryRepository : ITaskListsRepository
    {
        #region Fields

        public readonly List<TaskListEntity> TaskLists;
        public readonly List<TaskListShareEntity> TaskListShares;
        public readonly List<TaskListNoteEntity> TaskListNotes;
        public const string IdentityProvider = "windowsliveid";
        public const string User1RowKey = "user1-WindowsLiveID";
        public const string User2RowKey = "user2-WindowsLiveID";
        public const string User3RowKey = "user3-WindowsLiveID";
        public const string Note1PartitionKey = "user1-WindowsLiveID";
        public const string Note2PartitionKey = "user2-WindowsLiveID";
        public readonly string Note1RowKey = ShortGuid.NewGuid().ToString();
        public readonly string Note2RowKey = ShortGuid.NewGuid().ToString();
        public readonly string Note3RowKey = ShortGuid.NewGuid().ToString();
        public const string TaskList1PartitionKey = "user1-WindowsLiveID";
        public const string TaskList2PartitionKey = "user2-WindowsLiveID";
        public readonly string TaskList1RowKey = ShortGuid.NewGuid().ToString();
        public readonly string TaskList2RowKey = ShortGuid.NewGuid().ToString();
        public readonly string TaskList3RowKey = ShortGuid.NewGuid().ToString();

        #endregion Fields

        #region Constructors

        public TaskListsMemoryRepository()
        {
            TaskLists = new List<TaskListEntity>
                                     {
                                         new TaskListEntity(User1RowKey, TaskList1RowKey) { Title = "Title 1"}, 
                                         new TaskListEntity(User2RowKey, TaskList2RowKey) { Title = "Title 2"}
                                     };

            TaskListShares = new List<TaskListShareEntity>
                                                   {
                                                       new TaskListShareEntity(string.Format("{0}+{1}", TaskList1PartitionKey, TaskList1RowKey), User1RowKey),
                                                       new TaskListShareEntity(string.Format("{0}+{1}", TaskList1PartitionKey, TaskList1RowKey), User3RowKey),
                                                       new TaskListShareEntity(string.Format("{0}+{1}", TaskList2PartitionKey, TaskList2RowKey), User2RowKey),
                                                       new TaskListShareEntity(string.Format("{0}+{1}", TaskList2PartitionKey, TaskList2RowKey), User3RowKey)
                                                   };

            TaskListNotes = new List<TaskListNoteEntity>
                                 {
                                     new TaskListNoteEntity(string.Format("{0}+{1}", TaskList1PartitionKey, TaskList1RowKey), string.Format("{0}+{1}", Note1PartitionKey, Note1RowKey)),
                                     new TaskListNoteEntity(string.Format("{0}+{1}", TaskList1PartitionKey, TaskList1RowKey), string.Format("{0}+{1}", Note2PartitionKey, Note2RowKey))
                                 };
        }

        #endregion Constructors

        #region Public methods

        public IQueryable<TaskList> Load()
        {
            return TaskLists.Select(n => n.MapToTaskList()).AsQueryable();
        }

        public TaskList Get(string partitionKey, string rowKey)
        {
            var result = TaskLists.FirstOrDefault(n => n.PartitionKey == partitionKey && n.RowKey == rowKey);
            return result != null ? result.MapToTaskList() : null;
        }

        public TaskList Get(Expression<Func<TaskList, bool>> filter)
        {
            var result = TaskLists.Select(n => n.MapToTaskList()).AsQueryable().FirstOrDefault(filter);
            return result;
        }

        public void Create(TaskList entityToCreate)
        {
            var taskList = entityToCreate.MapToTaskListEntity();
            var taskListShare = new TaskListShareEntity(entityToCreate.RowKey, entityToCreate.Owner.RowKey);

            TaskLists.Add(taskList);
            TaskListShares.Add(taskListShare);
        }

        public void Update(TaskList entityToUpdate)
        {
            var taskList = entityToUpdate.MapToTaskListEntity();
            var taskListToRemove = TaskLists.First(n => n.PartitionKey == taskList.PartitionKey && n.RowKey == taskList.RowKey);

            TaskLists.Remove(taskListToRemove);
            TaskLists.Add(taskList);
        }

        public void Delete(TaskList entityToDelete)
        {
            TaskLists.RemoveAll(n => n.PartitionKey == entityToDelete.PartitionKey && n.RowKey == entityToDelete.RowKey);
            TaskListShares.RemoveAll(n => n.PartitionKey == entityToDelete.RowKey);
        }

        public IEnumerable<TaskList> GetShared(User user)
        {
            var taskLists = new List<TaskList>();
            var taskListShares = TaskListShares.Where(t => t.RowKey == user.RowKey);

            foreach (var taskListShare in taskListShares)
            {
                var taskListKeys = taskListShare.PartitionKey.Split('+');
                var taskListPartitionKey = taskListKeys[0];
                var taskListRowKey = taskListKeys[1];

                var taskList = TaskLists.FirstOrDefault(t => t.PartitionKey == taskListPartitionKey && t.RowKey == taskListRowKey);

                if (taskList != null)
                {
                    taskLists.Add(taskList.MapToTaskList());
                }
            }

            return taskLists.AsEnumerable();
        }

        public void AddShare(TaskList taskList, string userId)
        {
            TaskListShares.Add(new TaskListShareEntity(string.Format("{0}+{1}", taskList.PartitionKey, taskList.RowKey), userId));
        }

        public void RemoveShare(TaskList taskList, string userId)
        {
            TaskListShares.RemoveAll(n => n.PartitionKey == string.Format("{0}+{1}", taskList.PartitionKey, taskList.RowKey) && n.RowKey == userId);
        }

        public void LoadContainer(Note note)
        {
            var taskListNote = TaskListNotes.FirstOrDefault(tn => tn.RowKey == string.Format("{0}+{1}", note.PartitionKey, note.RowKey));

            if (taskListNote != null)
            {
                var taskListKeys = taskListNote.PartitionKey.Split('+');
                var taskListPartitionKey = taskListKeys[0];
                var taskListRowKey = taskListKeys[1];
                var taskList = TaskLists.FirstOrDefault(t => t.PartitionKey == taskListPartitionKey && t.RowKey == taskListRowKey);

                if (taskList != null)
                {
                    note.Container = taskList.MapToTaskList();   
                }
            }
        }

        public bool HasPermissionToEdit(User user, TaskList taskList)
        {
            var taskListSharePartitionKey = string.Format("{0}+{1}", taskList.PartitionKey, taskList.RowKey);
            var taskListShareRowkey = user.RowKey;

            return TaskListShares.Any(ns => ns.PartitionKey == taskListSharePartitionKey && ns.RowKey == taskListShareRowkey);
        }

        #endregion Public methods
    }
}