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

namespace CloudNotes.Repositories.Implementation
{
    /// <summary>
    /// Repository to manage all the actions related to the Notes, NoteShares and TaskListNotes Azure Tables.
    /// </summary>
    public class NotesRepository : INotesRepository
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;

        #endregion Fields

        #region Constructors

        public NotesRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Constructors

        #region Public methods

        /// <summary>
        /// Loads all the entities in the Notes Azure Table.
        /// </summary>
        /// <returns>IQueryable of all the Note entities</returns>
        public IQueryable<Note> Load()
        {
            return _unitOfWork.Load<NoteEntity>("Notes").ToList().Select(notesTableEntry => notesTableEntry.MapToNote()).AsQueryable();
        }

        /// <summary>
        /// Gets a single entity from the Notes Azure Table and maps it to a Note domain object.
        /// </summary>
        /// <param name="partitionKey">The Note partitionkey</param>
        /// <param name="rowKey">The Note rowkey</param>
        /// <returns>A Note if exists, null otherwise</returns>
        public Note Get(string partitionKey, string rowKey)
        {
            var result = _unitOfWork.Get<NoteEntity>("Notes", t => t.PartitionKey == partitionKey && t.RowKey == rowKey);
            return result != null ? result.MapToNote() : null;
        }

        /// <summary>
        /// Gets a single entity from the Notes Azure Table by filter and maps it to a Note domain object.
        /// </summary>
        /// <param name="filter">Lambda to filter the Note</param>
        /// <returns>A Note if exists, null otherwise</returns>
        public Note Get(Expression<Func<Note, bool>> filter)
        {
            return _unitOfWork.Get("Notes", filter);
        }

        /// <summary>
        /// Checks if a TaskList already has a Note with the provided title.
        /// </summary>
        /// <param name="title">The title to check if exists</param>
        /// <param name="container">The TaskList to check if a Note exists wiht the provided title</param>
        /// <returns>True if a Note already exists with the title, False otherwise</returns>
        public bool NoteWithTitleExists(string title, TaskList container)
        {
            var containerKeys = string.Format("{0}+{1}", container.PartitionKey, container.RowKey);
            var note = _unitOfWork.Get<Note>("Notes", n => n.ContainerKeys == containerKeys && n.Title == title);
            return note != null;
        }

        /// <summary>
        /// Creates a new Note.
        /// </summary>
        /// <param name="entityToCreate">Note domain object with the properties to map to an Notes table entity</param>
        public void Create(Note entityToCreate)
        {
            // get the last inserted Note ordering index
            var maxOrderingIndex = -1;
            var result = _unitOfWork.Load<NoteEntity>("Notes", n => n.PartitionKey == entityToCreate.Owner.RowKey).ToList();

            if (result.Any())
            {
                maxOrderingIndex = result.Max(n => n.OrderingIndex);
            }

            entityToCreate.PartitionKey = entityToCreate.Owner.RowKey;
            entityToCreate.RowKey = ShortGuid.NewGuid();
            entityToCreate.OrderingIndex = maxOrderingIndex + 1;
            entityToCreate.ContainerKeys = string.Format("{0}+{1}", entityToCreate.Container.PartitionKey, entityToCreate.Container.RowKey);

            var noteEntity = entityToCreate.MapToNoteEntity();
            _unitOfWork.Create(noteEntity, "Notes");

            // the User that creates the new Note is automatically add in the Note Shares list.
            var noteSharePartitionKey = string.Format("{0}+{1}", entityToCreate.PartitionKey, entityToCreate.RowKey);
            var noteShare = new NoteShareEntity(noteSharePartitionKey, entityToCreate.Owner.RowKey);
            _unitOfWork.Create(noteShare, "NoteShares");

            // store in the TaskListNotes an entity to indicate that the Note is in the containing TaskList.
            var taskListNoteEntityPartitionKey = string.Format("{0}+{1}", entityToCreate.Container.PartitionKey, entityToCreate.Container.RowKey);
            var taskListNoteEntityRowKey = string.Format("{0}+{1}", entityToCreate.PartitionKey, entityToCreate.RowKey);
            var taskListNoteEntity = new TaskListNoteEntity(taskListNoteEntityPartitionKey, taskListNoteEntityRowKey);
            _unitOfWork.Create(taskListNoteEntity, "TaskListNotes");
        }

        /// <summary>
        /// Updates an entity in the Notes Azure Table.
        /// </summary>
        /// <param name="entityToUpdate">Note domain object with the properties to update an existing Notes table entity</param>
        public void Update(Note entityToUpdate)
        {
            _unitOfWork.Update("Notes", entityToUpdate.MapToNoteEntity());
        }

        /// <summary>
        /// Deletes an entity from the Notes Azure Table and all the entities in the related Azure Tables.
        /// </summary>
        /// <param name="entityToDelete">TaskList domain object with the properties to delete an existing TaskLists table entity</param>
        public void Delete(Note entityToDelete)
        {
            _unitOfWork.Delete<NoteEntity>("Notes", entityToDelete.PartitionKey, entityToDelete.RowKey);

            // delete all the entities that relate a Containing TaskList and his Notes.
            var taskListNoteRowKey = string.Format("{0}+{1}", entityToDelete.PartitionKey, entityToDelete.RowKey);
            var taskListNote = _unitOfWork.Get<TaskListEntity>("TaskListNotes", tn => tn.RowKey == taskListNoteRowKey);
            _unitOfWork.Delete<TaskListEntity>("TaskListNotes", taskListNote.PartitionKey, taskListNote.RowKey);

            // delete all the entities related to the Note Shares list.
            var noteSharePartitionKey = string.Format("{0}+{1}", entityToDelete.PartitionKey, entityToDelete.RowKey);
            var shares = _unitOfWork.Load<NoteShareEntity>("NoteShares").Where(n => n.PartitionKey == noteSharePartitionKey);

            foreach (var share in shares)
            {
                _unitOfWork.Delete<NoteShareEntity>("NoteShares", share.PartitionKey, share.RowKey);
            }
        }

        /// <summary>
        /// Share the Note with a User.
        /// </summary>
        /// <param name="note">The Note to be shared</param>
        /// <param name="userId">RowKey from the User</param>
        public void AddShare(Note note, string userId)
        {
            var sharePartitionKey = string.Format("{0}+{1}", note.PartitionKey, note.RowKey);
            var share = new NoteShareEntity(sharePartitionKey, userId);
            _unitOfWork.Create(share, "NoteShares");
        }

        /// <summary>
        /// Remove an existing Share from the Note.
        /// </summary>
        /// <param name="note">The Note with the Share to be removed</param>
        /// <param name="userId">RowKey from the User</param>
        public void RemoveShare(Note note, string userId)
        {
            var sharePartitionKey = string.Format("{0}+{1}", note.PartitionKey, note.RowKey);
            _unitOfWork.Delete<NoteShareEntity>("NoteShares", sharePartitionKey, userId);
        }

        /// <summary>
        /// Loads and fills all the Notes for a TaskList.
        /// </summary>
        /// <param name="taskList">The TaskList to fill the Notes</param>
        public void LoadNotes(TaskList taskList)
        {
            var buffer = new List<Note>();
            var taskListNotePartitionKey = string.Format("{0}+{1}", taskList.PartitionKey, taskList.RowKey);
            var taskListNotes = _unitOfWork.Load<TaskListNoteEntity>("TaskListNotes", n => n.PartitionKey == taskListNotePartitionKey);

            foreach (var taskListNote in taskListNotes)
            {
                var noteKeys = taskListNote.RowKey.Split('+');
                var notePartitionKey = noteKeys[0];
                var noteRowKey = noteKeys[1];

                var note = Get(notePartitionKey, noteRowKey);

                if (note != null)
                {
                    buffer.Add(note);
                }
            }

            // insert the Notes maintaining the ascending order for the ordering index.
            foreach (var note in buffer.OrderBy(n => n.OrderingIndex))
            {
                taskList.Notes.Add(note);
            }
        }

        /// <summary>
        /// Checks if a User has permissions to edit a Note, if it is in the Note Share list.
        /// </summary>
        /// <param name="user">The User to check the permission</param>
        /// <param name="note">The Note the User wants to edit</param>
        /// <returns>True if the User is in the Note Share list, False otherwise</returns>
        public bool HasPermissionToEdit(User user, Note note)
        {
            var noteSharePartitionKey = string.Format("{0}+{1}", note.PartitionKey, note.RowKey);
            return _unitOfWork.Get<NoteShareEntity>("NoteShares", ns => ns.PartitionKey == noteSharePartitionKey && ns.RowKey == user.RowKey) != null;
        }

        #endregion Public methods
    }
}