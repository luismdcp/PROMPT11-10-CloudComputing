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

        public IQueryable<Note> Load()
        {
            return _unitOfWork.Load<NoteEntity>("Notes").ToList().Select(notesTableEntry => notesTableEntry.MapToNote()).AsQueryable();
        }

        public Note Get(string partitionKey, string rowKey)
        {
            var result = _unitOfWork.Get<NoteEntity>("Notes", t => t.PartitionKey == partitionKey && t.RowKey == rowKey);
            return result != null ? result.MapToNote() : null;
        }

        public Note Get(Expression<Func<Note, bool>> filter)
        {
            return _unitOfWork.Get("Notes", filter);
        }

        public bool NoteWithTitleExists(string title, TaskList container)
        {
            var containerKeys = string.Format("{0}+{1}", container.PartitionKey, container.RowKey);
            var note = _unitOfWork.Get<Note>("Notes", n => n.ContainerKeys == containerKeys && n.Title == title);
            return note != null;
        }

        public void Create(Note entityToCreate)
        {
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

            var noteSharePartitionKey = string.Format("{0}+{1}", entityToCreate.PartitionKey, entityToCreate.RowKey);
            var noteShare = new NoteShareEntity(noteSharePartitionKey, entityToCreate.Owner.RowKey);
            _unitOfWork.Create(noteShare, "NoteShares");

            var taskListNoteEntityPartitionKey = string.Format("{0}+{1}", entityToCreate.Container.PartitionKey, entityToCreate.Container.RowKey);
            var taskListNoteEntityRowKey = string.Format("{0}+{1}", entityToCreate.PartitionKey, entityToCreate.RowKey);
            var taskListNoteEntity = new TaskListNoteEntity(taskListNoteEntityPartitionKey, taskListNoteEntityRowKey);
            _unitOfWork.Create(taskListNoteEntity, "TaskListNotes");
        }

        public void Update(Note entityToUpdate)
        {
            _unitOfWork.Update("Notes", entityToUpdate.MapToNoteEntity());
        }

        public void Delete(Note entityToDelete)
        {
            _unitOfWork.Delete<NoteEntity>("Notes", entityToDelete.PartitionKey, entityToDelete.RowKey);

            var taskListNoteRowKey = string.Format("{0}+{1}", entityToDelete.PartitionKey, entityToDelete.RowKey);
            var taskListNote = _unitOfWork.Get<TaskListEntity>("TaskListNotes", tn => tn.RowKey == taskListNoteRowKey);
            _unitOfWork.Delete<TaskListEntity>("TaskListNotes", taskListNote.PartitionKey, taskListNote.RowKey);

            var noteSharePartitionKey = string.Format("{0}+{1}", entityToDelete.PartitionKey, entityToDelete.RowKey);
            var shares = _unitOfWork.Load<NoteShareEntity>("NoteShares").Where(n => n.PartitionKey == noteSharePartitionKey);

            foreach (var share in shares)
            {
                _unitOfWork.Delete<NoteShareEntity>("NoteShares", share.PartitionKey, share.RowKey);
            }
        }

        public void AddShare(Note note, string userId)
        {
            var sharePartitionKey = string.Format("{0}+{1}", note.PartitionKey, note.RowKey);
            var share = new NoteShareEntity(sharePartitionKey, userId);
            _unitOfWork.Create(share, "NoteShares");
        }

        public void RemoveShare(Note note, string userId)
        {
            var sharePartitionKey = string.Format("{0}+{1}", note.PartitionKey, note.RowKey);
            _unitOfWork.Delete<NoteShareEntity>("NoteShares", sharePartitionKey, userId);
        }

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

            foreach (var note in buffer.OrderBy(n => n.OrderingIndex))
            {
                taskList.Notes.Add(note);
            }
        }

        public bool HasPermissionToEdit(User user, Note note)
        {
            var noteSharePartitionKey = string.Format("{0}+{1}", note.PartitionKey, note.RowKey);
            return _unitOfWork.Get<NoteShareEntity>("NoteShares", ns => ns.PartitionKey == noteSharePartitionKey && ns.RowKey == user.RowKey) != null;
        }

        #endregion Public methods
    }
}