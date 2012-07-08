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
            var notesTableEntries = _unitOfWork.Load<NoteTableEntry>("Notes");
            return notesTableEntries.Select(noteTableEntry => noteTableEntry.MapToNote()).AsQueryable();
        }

        public Note Get(string partitionKey, string rowKey)
        {
            return _unitOfWork.Get<NoteTableEntry>("Notes", partitionKey, rowKey).MapToNote();
        }

        public void Create(Note entityToAdd)
        {
            var noteTableEntry = entityToAdd.MapToNoteTableEntry();
            _unitOfWork.Add(noteTableEntry, "Notes");

            var noteOwnerTableEntry = new NoteOwnerTableEntry(entityToAdd.Owner.RowKey, entityToAdd.RowKey);
            _unitOfWork.Add(noteOwnerTableEntry, "NoteOwner");
        }

        public void Update(Note entityToUpdate)
        {
            var noteTableEntry = entityToUpdate.MapToNoteTableEntry();
            _unitOfWork.Update(noteTableEntry);
        }

        public void Delete(Note entityToDelete)
        {
            var noteTableEntry = entityToDelete.MapToNoteTableEntry();
            _unitOfWork.Delete(noteTableEntry);
        }

        public void AddAssociatedUser(Note note, User userToAssociate)
        {
            var noteAssociatedUserTableEntry = new NoteAssociatedUserTableEntry(note.RowKey, userToAssociate.RowKey);
            _unitOfWork.Add(noteAssociatedUserTableEntry, "NoteAssociatedUsers");
        }

        public void DeleteAssociatedUser(Note note, User userToAssociate)
        {
            var noteAssociatedUserTableEntry = new NoteAssociatedUserTableEntry(note.RowKey, userToAssociate.RowKey);
            _unitOfWork.Delete(noteAssociatedUserTableEntry);
        }

        #endregion Public methods
    }
}