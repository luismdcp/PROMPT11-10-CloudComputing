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
            var taskLisTabletEntries = _unitOfWork.Load<TaskListTableEntry>("TaskLists");
            return taskLisTabletEntries.Select(taskListTableEntry => taskListTableEntry.MapToTaskList()).AsQueryable();
        }

        public TaskList Get(string partitionKey, string rowKey)
        {
            return _unitOfWork.Get<TaskListTableEntry>("TaskLists", partitionKey, rowKey).MapToTaskList();
        }

        public void Create(TaskList entityToAdd)
        {
            var taskListTableEntry = entityToAdd.MapToTaskListTableEntry();
            _unitOfWork.Add(taskListTableEntry, "TaskLists");
        }

        public void Update(TaskList entityToUpdate)
        {
            var taskListTableEntry = entityToUpdate.MapToTaskListTableEntry();
            _unitOfWork.Update(taskListTableEntry);
        }

        public void Delete(TaskList entityToDelete)
        {
            var taskListTableEntry = entityToDelete.MapToTaskListTableEntry();
            _unitOfWork.Delete(taskListTableEntry);
        }

        public void AddAssociatedUser(TaskList taskList, User userToAssociate)
        {
            var taskListAssociatedUsersTableEntry = new TaskListAssociatedUserTableEntry(taskList.PartitionKey, taskList.RowKey);
            _unitOfWork.Add(taskListAssociatedUsersTableEntry, "TaskListAssociatedUsers");
        }

        public void DeleteAssociatedUser(TaskList taskList, User userToAssociate)
        {
            var taskListAssociatedUsersTableEntry = new TaskListAssociatedUserTableEntry(taskList.PartitionKey, taskList.RowKey);
            _unitOfWork.Delete(taskListAssociatedUsersTableEntry);
        }

        #endregion Public methods
    }
}