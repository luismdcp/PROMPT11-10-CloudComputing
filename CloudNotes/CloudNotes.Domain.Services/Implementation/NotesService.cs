using System;
using System.Linq;
using System.Linq.Expressions;
using CloudNotes.Domain.Entities;
using CloudNotes.Domain.Services.Contracts;
using CloudNotes.Repositories.Contracts;
using CloudNotes.Repositories.Helpers;

namespace CloudNotes.Domain.Services.Implementation
{
    public class NotesService : INotesService
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly INotesRepository _repository;
        private readonly ITaskListsService _taskListsService;

        #endregion Fields

        #region Constructors

        public NotesService(IUnitOfWork unitOfWork, INotesRepository notesRepository, ITaskListsService taskListsService)
        {
            _unitOfWork = unitOfWork;
            _repository = notesRepository;
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
            var note = _repository.Get(partitionKey, rowKey);
            _taskListsService.LoadContainer(note);
            return note;
        }

        public Note Get(Expression<Func<Note, bool>> filter)
        {
            var note = _repository.Get(filter);
            _taskListsService.LoadContainer(note);
            return note;
        }

        public bool NoteWithTitleExists(string title, TaskList container)
        {
            return _repository.NoteWithTitleExists(title, container);
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

        public void AddShare(Note note, string userId)
        {
            _repository.AddShare(note, userId);
            _unitOfWork.SubmitChanges();
        }

        public void RemoveShare(Note note, string userId)
        {
            _repository.RemoveShare(note, userId);
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
                                   PartitionKey = note.PartitionKey,
                                   RowKey = ShortGuid.NewGuid(),
                                   IsClosed = note.IsClosed,
                                   OrderingIndex = note.OrderingIndex,
                                   Owner = note.Owner,
                                   Title = note.Title,
                                   Content = note.Content,
                                   Container = taskListDestination
                               };

            _repository.Create(noteCopy);

            foreach (var share in note.Share)
            {
                if (share.RowKey != note.Owner.RowKey)
                {
                    _repository.AddShare(noteCopy, share.RowKey);
                }
            }

            _unitOfWork.SubmitChanges();
        }

        public void MoveNote(Note note, TaskList taskListDestination)
        {
            _repository.Delete(note);
            CopyNote(note, taskListDestination);
        }

        public bool HasPermissionToEdit(User user, Note note)
        {
            return _repository.HasPermissionToEdit(user, note);
        }

        #endregion Public methods
    }
}