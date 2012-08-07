using System.Collections.Generic;
using System.Linq;
using CloudNotes.Domain.Entities;
using CloudNotes.Repositories.Contracts;
using CloudNotes.Repositories.Entities;
using CloudNotes.Repositories.Entities.Relation;
using CloudNotes.Repositories.Extensions;

namespace CloudNotes.Tests.Domain.Services.Memory
{
    public class TaskListsMemoryRepository : ITaskListsRepository
    {
        #region Fields

        private readonly List<TaskListTableEntry> _taskListsTableEntries;
        private readonly List<TaskListOwnerTableEntry> _taskListOwnerTableEntries;
        private readonly List<TaskListAssociatedUserTableEntry> _taskListAssociatedUsersTableEntries;

        #endregion Fields

        #region Constructors

        public TaskListsMemoryRepository()
        {
            _taskListsTableEntries = new List<TaskListTableEntry>
                                     {
                                         new TaskListTableEntry("user1", "taskList1") { Title = "Title 1"}, 
                                         new TaskListTableEntry("user2", "taskList2") { Title = "Title 2"}
                                     };

            _taskListOwnerTableEntries = new List<TaskListOwnerTableEntry>
                                         {
                                             new TaskListOwnerTableEntry("user1", "taskList1"),
                                             new TaskListOwnerTableEntry("user2", "taskList2")
                                         };

            _taskListAssociatedUsersTableEntries = new List<TaskListAssociatedUserTableEntry>
                                                   {
                                                       new TaskListAssociatedUserTableEntry("taskList1", "user1"),
                                                       new TaskListAssociatedUserTableEntry("taskList1", "user3"),
                                                       new TaskListAssociatedUserTableEntry("taskList2", "user2"),
                                                       new TaskListAssociatedUserTableEntry("taskList2", "user3")
                                                   };
        }

        #endregion Constructors

        #region Public methods

        public IQueryable<TaskList> Load()
        {
            return _taskListsTableEntries.Select(n => n.MapToTaskList()).AsQueryable();
        }

        public TaskList Get(string partitionKey, string rowKey)
        {
            var result = _taskListsTableEntries.FirstOrDefault(n => n.PartitionKey == partitionKey && n.RowKey == rowKey);
            return result != null ? result.MapToTaskList() : null;
        }

        public void Create(TaskList entityToCreate)
        {
            var taskListTableEntry = entityToCreate.MapToTaskListTableEntry();
            var taskListOwnerTableEntry = new TaskListOwnerTableEntry(entityToCreate.Owner.RowKey, entityToCreate.RowKey);
            var taskListAssociatedUserTableEntry = new TaskListAssociatedUserTableEntry(entityToCreate.RowKey, entityToCreate.Owner.RowKey);

            _taskListsTableEntries.Add(taskListTableEntry);
            _taskListOwnerTableEntries.Add(taskListOwnerTableEntry);
            _taskListAssociatedUsersTableEntries.Add(taskListAssociatedUserTableEntry);
        }

        public void Update(TaskList entityToUpdate)
        {
            var taskListTableEntryToUpdate = _taskListsTableEntries.First(n => n.PartitionKey == entityToUpdate.PartitionKey && n.RowKey == entityToUpdate.RowKey);
            taskListTableEntryToUpdate.Title = entityToUpdate.Title;
        }

        public void Delete(TaskList entityToDelete)
        {
            _taskListsTableEntries.RemoveAll(n => n.PartitionKey == entityToDelete.PartitionKey && n.RowKey == entityToDelete.RowKey);
            _taskListOwnerTableEntries.RemoveAll(n => n.RowKey == entityToDelete.RowKey);
            _taskListAssociatedUsersTableEntries.RemoveAll(n => n.PartitionKey == entityToDelete.RowKey);
        }

        public IEnumerable<TaskList> GetTaskListsAssociatedByUser(User user)
        {
            var taskLists = new List<TaskList>();
            var taskListsAssociatedToUserEntries = _taskListAssociatedUsersTableEntries.Where(t => t.RowKey == user.RowKey);

            foreach (var taskListAssociatedUserTableEntry in taskListsAssociatedToUserEntries)
            {
                var taskList = _taskListsTableEntries.FirstOrDefault(t => t.RowKey == taskListAssociatedUserTableEntry.PartitionKey);

                if (taskList != null)
                {
                    taskLists.Add(taskList.MapToTaskList());
                }
            }

            return taskLists.AsEnumerable();
        }

        public void AddAssociatedUser(TaskList taskList, User associatedUser)
        {
            _taskListAssociatedUsersTableEntries.Add(new TaskListAssociatedUserTableEntry(taskList.RowKey, associatedUser.RowKey));
        }

        public void RemoveAssociatedUser(TaskList taskList, User associatedUser)
        {
            _taskListAssociatedUsersTableEntries.RemoveAll(n => n.PartitionKey == taskList.RowKey && n.RowKey == associatedUser.RowKey);
        }

        public void CopyNote(TaskList taskListSource, TaskList taskListDestination, Note note)
        {
            var newNote = new Note(taskListDestination.RowKey, note.RowKey)
            {
                IsClosed = note.IsClosed,
                OrderingIndex = note.OrderingIndex,
                Owner = note.Owner,
                Title = note.Title,
                ContainerList = taskListDestination
            };

            taskListDestination.Notes.Add(newNote);
        }

        public void MoveNote(TaskList taskListSource, TaskList taskListDestination, Note note)
        {
            taskListSource.Notes.Remove(note);
            note.ContainerList = taskListDestination;
            taskListDestination.Notes.Add(note);
        }

        #endregion Public methods
    }
}