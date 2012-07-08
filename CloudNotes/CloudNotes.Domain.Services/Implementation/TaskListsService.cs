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
        private readonly INotesRepository _noteRepository;

        #endregion Fields

        #region Constructors

        public TaskListsService(IUnitOfWork unitOfWork, ITaskListsRepository taskListsRepository, INotesRepository notesRepository)
        {
            _unitOfWork = unitOfWork;
            _taskListsRepository = taskListsRepository;
            _noteRepository = notesRepository;
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

        public void Add(TaskList entityToAdd)
        {
            _taskListsRepository.Create(entityToAdd);
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

        public void AddAssociatedUser(TaskList taskList, User associatedUser)
        {
            taskList.AssociatedUsers.Add(associatedUser);
            _taskListsRepository.AddAssociatedUser(taskList, associatedUser);
            _unitOfWork.SubmitChanges();
        }

        public void DeleteAssociatedUser(TaskList taskList, User associatedUser)
        {
            taskList.AssociatedUsers.Remove(associatedUser);
            _taskListsRepository.DeleteAssociatedUser(taskList, associatedUser);
            _unitOfWork.SubmitChanges();
        }

        public void CopyNote(TaskList taskListSource, TaskList taskListDestination, Note note)
        {
            note.PartitionKey = taskListDestination.RowKey;
            _noteRepository.Create(note);
            _unitOfWork.SubmitChanges();
        }

        public void MoveNote(TaskList taskListSource, TaskList taskListDestination, Note note)
        {
            var newNote = new Note(taskListDestination.RowKey, note.RowKey)
                              {
                                  IsClosed = note.IsClosed,
                                  OrderingIndex = note.OrderingIndex,
                                  Owner = note.Owner,
                                  Title = note.Title,
                                  ContainerList = taskListDestination
                              };

            _noteRepository.Delete(note);
            _noteRepository.Create(newNote);
            _unitOfWork.SubmitChanges();
        }

        #endregion Public methods
    }
}