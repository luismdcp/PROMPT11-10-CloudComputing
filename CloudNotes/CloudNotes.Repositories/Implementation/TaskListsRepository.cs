using System;
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

        private readonly TableDataContext _unitOfWork;
        private readonly TableRepository<TaskListTableEntry> _taskListsTableRepository;
        private readonly TableRepository<TaskListAssociatedUserTableEntry> _taskListAssociatedUsersTableRepository;

        #endregion Fields

        #region Constructors

        public TaskListsRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork as TableDataContext;
            _taskListsTableRepository = new TableRepository<TaskListTableEntry>("TaskLists", _unitOfWork);
            _taskListAssociatedUsersTableRepository = new TableRepository<TaskListAssociatedUserTableEntry>("TaskListAssociatedUsers", _unitOfWork);
        }

        #endregion Constructors

        #region Public methods

        public IQueryable<TaskList> Load()
        {
            var taskLisTabletEntries = _taskListsTableRepository.Load().ToList();
            return taskLisTabletEntries.Select(taskListTableEntry => taskListTableEntry.MapToTaskList()).AsQueryable();
        }

        public TaskList Get(string partitionKey, string rowKey)
        {
            return _taskListsTableRepository.Get(partitionKey, rowKey).MapToTaskList();
        }

        public void Add(TaskList entityToAdd)
        {
            var taskListTableEntry = entityToAdd.MapToTaskListTableEntry();
            _taskListsTableRepository.Add(taskListTableEntry);
        }

        public void Update(TaskList entityToUpdate)
        {
            var taskListTableEntry = entityToUpdate.MapToTaskListTableEntry();
            _taskListsTableRepository.Update(taskListTableEntry);
        }

        public void Delete(TaskList entityToDelete)
        {
            var taskListTableEntry = entityToDelete.MapToTaskListTableEntry();
            _taskListsTableRepository.Delete(taskListTableEntry);
        }

        public void AddAssociatedUser(TaskList taskList, User associatedUser)
        {
            var taskListAssociatedUsersTableEntry = new TaskListAssociatedUserTableEntry(taskList.PartitionKey, taskList.RowKey);
            _taskListAssociatedUsersTableRepository.Add(taskListAssociatedUsersTableEntry);
        }

        public void DeleteAssociatedUser(TaskList taskList, User associatedUser)
        {
            var taskListAssociatedUsersTableEntry = new TaskListAssociatedUserTableEntry(taskList.PartitionKey, taskList.RowKey);
            _taskListAssociatedUsersTableRepository.Delete(taskListAssociatedUsersTableEntry);
        }

        public void AddOrReplaceAssociatedUsers(TaskList taskList)
        {
            throw new NotImplementedException();
        }

        #endregion Public methods
    }
}