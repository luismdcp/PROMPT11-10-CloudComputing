using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        public IQueryable<TaskList> Load()
        {
            return _taskListsRepository.Load();
        }

        public TaskList Get(string partitionKey, string rowKey)
        {
            return _taskListsRepository.Get(partitionKey, rowKey);
        }

        public TaskList Get(Expression<Func<TaskList, bool>> filter)
        {
            return _taskListsRepository.Get(filter);
        }

        public TaskList Get(string combinedKeys)
        {
            var taskListKeys = combinedKeys.Split('+');
            var taskListPartitionKey = taskListKeys[0];
            var taskListRowKey = taskListKeys[1];
            return Get(taskListPartitionKey, taskListRowKey);
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
            _notesRepository.LoadNotes(entityToDelete);

            foreach (var note in entityToDelete.Notes)
            {
                _notesRepository.Delete(note);
            }

            _taskListsRepository.Delete(entityToDelete);

            _unitOfWork.SubmitChanges();
        }

        public IEnumerable<TaskList> GetShared(User user)
        {
            return _taskListsRepository.GetShared(user);
        }

        public void LoadContainer(Note note)
        {
            _taskListsRepository.LoadContainer(note);
        }

        public void AddShare(TaskList taskList, string userId)
        {
            _taskListsRepository.AddShare(taskList, userId);
            _unitOfWork.SubmitChanges();
        }

        public void RemoveShare(TaskList taskList, string userId)
        {
            _taskListsRepository.RemoveShare(taskList, userId);
            _unitOfWork.SubmitChanges();
        }

        public bool HasPermissionToEdit(User user, TaskList taskList)
        {
            return _taskListsRepository.HasPermissionToEdit(user, taskList);
        }

        #endregion Public methods
    }
}