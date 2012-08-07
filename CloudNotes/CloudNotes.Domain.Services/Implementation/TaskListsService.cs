using System.Collections.Generic;
using System.Linq;
using CloudNotes.Domain.Entities;
using CloudNotes.Domain.Services.Contracts;
using CloudNotes.Repositories.Contracts;

namespace CloudNotes.Domain.Services.Implementation
{
    public class TaskListsService : ITaskListsService
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly ITaskListsRepository _taskListsRepository;
        private readonly INotesRepository _notesRepository;
        private readonly IUsersService _usersService;

        #endregion Fields

        #region Constructors

        public TaskListsService(IUnitOfWork unitOfWork, ITaskListsRepository taskListsRepository, INotesRepository notesRepository, IUsersService usersService)
        {
            _unitOfWork = unitOfWork;
            _taskListsRepository = taskListsRepository;
            _notesRepository = notesRepository;
            _usersService = usersService;
        }

        #endregion Constructors

        #region Public methods

        public IQueryable<TaskList> Load()
        {
            return _taskListsRepository.Load();
        }

        public TaskList Get(string partitionKey, string rowKey)
        {
            return _taskListsRepository.Get(partitionKey, rowKey);
        }

        public TaskList GetByTitleAndOwner(string title, User owner)
        {
            return _taskListsRepository.GetByTitleAndOwner(title, owner);
        }

        public TaskList GetTaskListEagerLoaded(string taskListTitle, User user)
        {
            var taskList = GetByTitleAndOwner(taskListTitle.Replace('-', ' '), user);
            _usersService.LoadTaskListOwner(taskList);
            _usersService.LoadTaskListAssociatedUsers(taskList);

            return taskList;
        }

        public void Create(TaskList entityToCreate)
        {
            _taskListsRepository.Create(entityToCreate);
            _unitOfWork.SubmitChanges();
        }

        public void Update(TaskList entityToUpdate)
        {
            _taskListsRepository.Update(entityToUpdate);
            _unitOfWork.SubmitChanges();
        }

        public void Delete(TaskList entityToDelete)
        {
            _taskListsRepository.Delete(entityToDelete);
            _unitOfWork.SubmitChanges();
        }

        public IEnumerable<TaskList> GetTaskListsUserIsAssociated(User user)
        {
            return _taskListsRepository.GetTaskListsAssociatedByUser(user);
        }

        public IEnumerable<TaskList> GetTaskListsOwnedByUser(User user)
        {
            return _taskListsRepository.GetTaskListsOwnedByUser(user);
        }

        public void AddAssociatedUser(TaskList taskList, User associatedUser)
        {
            taskList.AssociatedUsers.Add(associatedUser);
            _taskListsRepository.AddAssociatedUser(taskList, associatedUser);
            _unitOfWork.SubmitChanges();
        }

        public void RemoveAssociatedUser(TaskList taskList, User associatedUser)
        {
            taskList.AssociatedUsers.Remove(associatedUser);
            _taskListsRepository.RemoveAssociatedUser(taskList, associatedUser);
            _unitOfWork.SubmitChanges();
        }

        #endregion Public methods
    }
}