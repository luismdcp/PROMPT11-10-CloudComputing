using System;
using System.Linq;
using CloudNotes.Domain.Entities;
using CloudNotes.Repositories.Contracts;
using CloudNotes.Repositories.Entities;
using CloudNotes.Repositories.Entities.Relation;
using CloudNotes.Repositories.Extensions;

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
            return _unitOfWork.Load<NoteTableEntry>("Notes").ToList().Select(notesTableEntry => notesTableEntry.MapToNote()).AsQueryable();
        }

        public Note Get(string partitionKey, string rowKey)
        {
            var result = _unitOfWork.Get<NoteTableEntry>("Notes", partitionKey, rowKey);

            if (result != null)
            {
                return result.MapToNote();
            }

            return null;
        }

        public Note GetByTitle(string title, TaskList containerList)
        {
            var result = _unitOfWork.Load<NoteTableEntry>("Notes").Where(n => n.PartitionKey == containerList.RowKey && n.Title == title).FirstOrDefault();

            if (result != null)
            {
                var note = result.MapToNote();
                note.ContainerList = containerList;
                return note;
            }

            return null;
        }

        public Note GetByTitle(string title, string containerTaskListRowKey)
        {
            var result = _unitOfWork.Load<NoteTableEntry>("Notes").Where(n => n.PartitionKey == containerTaskListRowKey && n.Title == title).FirstOrDefault();
            return result != null ? result.MapToNote() : null;
        }

        public void Create(Note entityToCreate)
        {
            var maxOrderingIndex = -1;
            var result = _unitOfWork.Load<NoteTableEntry>("Notes").Where(n => n.PartitionKey == entityToCreate.ContainerList.RowKey).ToList().OrderByDescending(n => n.OrderingIndex).Take(1);

            if (result.Count() == 1)
            {
                maxOrderingIndex = result.ToList()[0].OrderingIndex;
            }

            entityToCreate.PartitionKey = entityToCreate.ContainerList.RowKey;
            entityToCreate.RowKey = Guid.NewGuid().ToString();
            entityToCreate.OrderingIndex = maxOrderingIndex + 1;

            var noteTableEntry = entityToCreate.MapToNoteTableEntry();
            _unitOfWork.Create(noteTableEntry, "Notes");

            var noteOwnerTableEntry = new NoteOwnerTableEntry(entityToCreate.Owner.RowKey, entityToCreate.RowKey);
            _unitOfWork.Create(noteOwnerTableEntry, "NoteOwner");

            var noteAssociatedUserTableEntry = new NoteAssociatedUserTableEntry(entityToCreate.RowKey, entityToCreate.Owner.RowKey);
            _unitOfWork.Create(noteAssociatedUserTableEntry, "NoteAssociatedUsers");
        }

        public void Update(Note entityToUpdate)
        {
            _unitOfWork.Update("Notes", entityToUpdate.MapToNoteTableEntry());
        }

        public void Delete(Note entityToDelete)
        {
            _unitOfWork.Delete<NoteTableEntry>("Notes", entityToDelete.PartitionKey, entityToDelete.RowKey);
            _unitOfWork.Delete<NoteOwnerTableEntry>("NoteOwner", entityToDelete.Owner.PartitionKey, entityToDelete.RowKey);

            var associatedUsers = _unitOfWork.Load<NoteAssociatedUserTableEntry>("NoteAssociatedUsers").Where(n => n.PartitionKey == entityToDelete.RowKey);

            foreach (var noteAssociatedUserTableEntry in associatedUsers)
            {
                _unitOfWork.Delete<NoteAssociatedUserTableEntry>("NoteAssociatedUsers", noteAssociatedUserTableEntry.PartitionKey, noteAssociatedUserTableEntry.RowKey);
            }
        }

        public void AddAssociatedUser(Note note, User userToAdd)
        {
            var noteAssociatedUserTableEntry = new NoteAssociatedUserTableEntry(note.RowKey, userToAdd.RowKey);
            _unitOfWork.Create(noteAssociatedUserTableEntry, "NoteAssociatedUsers");
        }

        public void RemoveAssociatedUser(Note note, User userToRemove)
        {
            _unitOfWork.Delete<NoteAssociatedUserTableEntry>("NoteAssociatedUsers", note.RowKey, userToRemove.RowKey);
        }

        public void LoadNotes(TaskList taskList)
        {
            var notes = _unitOfWork.Load<NoteTableEntry>("Notes").Where(n => n.PartitionKey == taskList.RowKey).ToList().OrderBy(t => t.OrderingIndex);

            foreach (var noteTableEntry in notes)
            {
                taskList.Notes.Add(noteTableEntry.MapToNote());
            }
        }

        #endregion Public methods
    }
}