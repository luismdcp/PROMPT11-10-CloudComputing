using System;
using System.Linq;
using System.Linq.Expressions;
using CloudNotes.Domain.Entities;
using CloudNotes.Domain.Services.Contracts;
using CloudNotes.Repositories.Contracts;
using CloudNotes.Repositories.Helpers;

namespace CloudNotes.Domain.Services.Implementation
{
    /// <summary>
    /// Service to manage all operations related to Notes.
    /// </summary>
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

        /// <summary>
        /// Load all Notes.
        /// </summary>
        /// <returns>IQueryable of all Notes</returns>
        public IQueryable<Note> Load()
        {
            return _repository.Load();
        }

        /// <summary>
        /// Get a Note by his partiton key and row key.
        /// </summary>
        /// <param name="partitionKey">The Note partition key</param>
        /// <param name="rowKey">The Note row key</param>
        /// <returns>A Note if exists, null otherwise</returns>
        public Note Get(string partitionKey, string rowKey)
        {
            var note = _repository.Get(partitionKey, rowKey);
            _taskListsService.LoadContainer(note);
            return note;
        }

        /// <summary>
        /// Get a Note by a filter.
        /// </summary>
        /// <param name="filter">Lambda expression filter</param>
        /// <returns>A Note if exists, null otherwise</returns>
        public Note Get(Expression<Func<Note, bool>> filter)
        {
            var note = _repository.Get(filter);
            _taskListsService.LoadContainer(note);
            return note;
        }

        /// <summary>
        /// Chek if a Note with the title already exists in the containing TaskList.
        /// </summary>
        /// <param name="title">Title to check if exists</param>
        /// <param name="container">The Note containing TaskList</param>
        /// <returns>True if a Note already exists with the same title, False otherwise</returns>
        public bool NoteWithTitleExists(string title, TaskList container)
        {
            return _repository.NoteWithTitleExists(title, container);
        }

        /// <summary>
        /// Create a Note.
        /// </summary>
        /// <param name="entityToCreate">Note to create</param>
        public void Create(Note entityToCreate)
        {
            _repository.Create(entityToCreate);
            _unitOfWork.SubmitChanges();
        }

        /// <summary>
        /// Update a Note.
        /// </summary>
        /// <param name="entityToUpdate">Note to update</param>
        public void Update(Note entityToUpdate)
        {
            _repository.Update(entityToUpdate);
            _unitOfWork.SubmitChanges();
        }

        /// <summary>
        /// Delete a Note.
        /// </summary>
        /// <param name="entityToDelete">Note to delete</param>
        public void Delete(Note entityToDelete)
        {
            _repository.Delete(entityToDelete);
            _unitOfWork.SubmitChanges();
        }

        /// <summary>
        /// Share the Note with a User.
        /// </summary>
        /// <param name="note">Note to be shared</param>
        /// <param name="userId">The User row key</param>
        public void AddShare(Note note, string userId)
        {
            _repository.AddShare(note, userId);
            _unitOfWork.SubmitChanges();
        }

        /// <summary>
        /// Remove the User from the Note Share list.
        /// </summary>
        /// <param name="note">Note to remove the Share</param>
        /// <param name="userId">The User row key</param>
        public void RemoveShare(Note note, string userId)
        {
            _repository.RemoveShare(note, userId);
            _unitOfWork.SubmitChanges();
        }

        /// <summary>
        /// Load all the Notes in the TaskList.
        /// </summary>
        /// <param name="taskList">TaskList to load the Notes</param>
        public void LoadNotes(TaskList taskList)
        {
            _repository.LoadNotes(taskList);
        }

        /// <summary>
        /// Copy a Note to another TaskList.
        /// </summary>
        /// <param name="note">Note to be copied</param>
        /// <param name="taskListDestination">TaskList where the Note will be copied</param>
        /// <returns>The Note's Copy new row key</returns>
        public string CopyNote(Note note, TaskList taskListDestination)
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

            // copy the original Note share list to the copied Note share list.
            foreach (var share in note.Share)
            {
                if (share.RowKey != note.Owner.RowKey)
                {
                    _repository.AddShare(noteCopy, share.RowKey);
                }
            }

            _unitOfWork.SubmitChanges();
            return noteCopy.RowKey;
        }

        /// <summary>
        /// Move a Note to another TaskList.
        /// </summary>
        /// <param name="note">Note to be moved</param>
        /// <param name="taskListDestination">TaskList where the Note will be moved</param>
        /// <returns></returns>
        public string MoveNote(Note note, TaskList taskListDestination)
        {
            _repository.Delete(note);
            return CopyNote(note, taskListDestination);
        }

        /// <summary>
        /// Check if the User has permission to edit the Note.
        /// </summary>
        /// <param name="user">User to check the permissio</param>
        /// <param name="note">Note to be edited</param>
        /// <returns>True if the User has permission to edit the Note, False otherwise</returns>
        public bool HasPermissionToEdit(User user, Note note)
        {
            return _repository.HasPermissionToEdit(user, note);
        }

        #endregion Public methods
    }
}