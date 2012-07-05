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
        private readonly TableRepository<NoteTableEntry> _notesTableRepository;
        private readonly TableRepository<NoteOwnerTableEntry> _noteOwnerTableRepository;
        private readonly TableRepository<NoteAssociatedUserTableEntry> _noteAssociatedUsersTableRepository;

        #endregion Fields

        #region Constructors

        public NotesRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _notesTableRepository = new TableRepository<NoteTableEntry>("Notes", _unitOfWork);
            _noteOwnerTableRepository = new TableRepository<NoteOwnerTableEntry>("NoteOwner", _unitOfWork);
            _noteAssociatedUsersTableRepository = new TableRepository<NoteAssociatedUserTableEntry>("NoteAssociatedUsers", _unitOfWork);
        }

        #endregion Constructors

        #region Public methods

        public IQueryable<Note> Load()
        {
            var noteEntries = _notesTableRepository.Load().ToList();
            return noteEntries.Select(noteTableEntry => noteTableEntry.MapToNote()).AsQueryable();
        }

        public Note Get(string partitionKey, string rowKey)
        {
            return _notesTableRepository.Get(partitionKey, rowKey).MapToNote();
        }

        public void Add(Note entityToAdd)
        {
            var noteTableEntry = entityToAdd.MapToNoteTableEntry();
            _notesTableRepository.Add(noteTableEntry);

            var noteOwnerTableEntry = new NoteOwnerTableEntry(entityToAdd.Owner.RowKey, entityToAdd.RowKey);
            _noteOwnerTableRepository.Add(noteOwnerTableEntry);
        }

        public void Update(Note entityToUpdate)
        {
            var noteEntry = entityToUpdate.MapToNoteTableEntry();
            _notesTableRepository.Update(noteEntry);
        }

        public void Delete(Note entityToDelete)
        {
            var noteEntry = entityToDelete.MapToNoteTableEntry();
            _notesTableRepository.Delete(noteEntry);
        }

        public void AddAssociatedUser(Note note, User associatedUser)
        {
            var noteAssociatedUserTableEntry = new NoteAssociatedUserTableEntry(note.RowKey, associatedUser.RowKey);
            _noteAssociatedUsersTableRepository.Add(noteAssociatedUserTableEntry);
        }

        public void DeleteAssociatedUser(Note note, User associatedUser)
        {
            var noteAssociatedUserTableEntry = new NoteAssociatedUserTableEntry(note.RowKey, associatedUser.RowKey);
            _noteAssociatedUsersTableRepository.Delete(noteAssociatedUserTableEntry);
        }

        #endregion Public methods
    }
}