using System.Linq;
using CloudNotes.Domain.Entities;
using CloudNotes.Domain.Services.Contracts;
using CloudNotes.Repositories.Contracts;

namespace CloudNotes.Domain.Services.Implementation
{
    public class NotesService : INotesService
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly INotesRepository _repository;
        private readonly IUsersService _usersService;
        private readonly ITaskListsService _taskListsService;

        #endregion Fields

        #region Constructors

        public NotesService(IUnitOfWork unitOfWork, INotesRepository notesRepository, IUsersService usersService, ITaskListsService taskListsService)
        {
            _unitOfWork = unitOfWork;
            _repository = notesRepository;
            _usersService = usersService;
            _taskListsService = taskListsService;
        }

        #endregion Constructors

        #region Public methods

        public IQueryable<Note> Load()
        {
            return _repository.Load();
        }

        public Note Get(string partitionKey, string rowKey)
        {
            return _repository.Get(partitionKey, rowKey);
        }

        public Note GetByTitle(string title, TaskList containerList)
        {
            return _repository.GetByTitle(title, containerList);
        }

        public Note GetByTitle(string title, string containerTaskListRowKey)
        {
            return _repository.GetByTitle(title, containerTaskListRowKey);
        }

        public Note GetNoteEagerLoaded(string taskListTitle, string noteTitle, User user)
        {
            var taskList = _taskListsService.GetByTitleAndOwner(taskListTitle.Replace('-', ' '), user);
            var note = GetByTitle(noteTitle.Replace('-', ' '), taskList);
            _usersService.LoadNoteOwner(note);
            _usersService.LoadNoteAssociatedUsers(note);

            return note;
        }

        public void Create(Note entityToCreate)
        {
            _repository.Create(entityToCreate);
            _unitOfWork.SubmitChanges();
        }

        public void Update(Note entityToUpdate)
        {
            _repository.Update(entityToUpdate);
            _unitOfWork.SubmitChanges();
        }

        public void Delete(Note entityToDelete)
        {
            _repository.Delete(entityToDelete);
            _unitOfWork.SubmitChanges();
        }

        public void AddAssociatedUser(Note note, User associatedUser)
        {
            note.AssociatedUsers.Add(associatedUser);
            _repository.AddAssociatedUser(note, associatedUser);
            _unitOfWork.SubmitChanges();
        }

        public void RemoveAssociatedUser(Note note, User associatedUser)
        {
            note.AssociatedUsers.Remove(associatedUser);
            _repository.RemoveAssociatedUser(note, associatedUser);
            _unitOfWork.SubmitChanges();
        }

        public void LoadNotes(TaskList taskList)
        {
            _repository.LoadNotes(taskList);
        }

        public void CopyNote(Note note, TaskList taskListDestination)
        {
            var noteCopy = new Note
            {
                PartitionKey = taskListDestination.RowKey,
                RowKey = note.RowKey,
                IsClosed = note.IsClosed,
                OrderingIndex = note.OrderingIndex,
                Owner = note.Owner,
                Title = note.Title,
                Content = note.Content,
                ContainerList = taskListDestination
            };

            _repository.Create(noteCopy);
            _unitOfWork.SubmitChanges();
        }

        public void MoveNote(Note note, TaskList taskListDestination)
        {
            var newNote = new Note
            {
                PartitionKey = taskListDestination.RowKey,
                RowKey = note.RowKey,
                IsClosed = note.IsClosed,
                OrderingIndex = note.OrderingIndex,
                Owner = note.Owner,
                Title = note.Title,
                ContainerList = taskListDestination
            };

            _repository.Delete(note);
            _repository.Create(newNote);
            _unitOfWork.SubmitChanges();
        }

        #endregion Public methods
    }
}