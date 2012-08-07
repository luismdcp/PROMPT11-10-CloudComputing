using System.Collections.Generic;
using System.Linq;
using CloudNotes.Domain.Entities;
using CloudNotes.Repositories.Contracts;
using CloudNotes.Repositories.Entities;
using CloudNotes.Repositories.Entities.Relation;
using CloudNotes.Repositories.Extensions;

namespace CloudNotes.Tests.Domain.Services.Memory
{
    public class NotesMemoryRepository : INotesRepository
    {
        #region Fields

        private readonly List<NoteTableEntry> _notesTableEntries;
        private readonly List<NoteOwnerTableEntry> _noteOwnerTableEntries;
        private readonly List<NoteAssociatedUserTableEntry> _noteAssociatedUsersTableEntries;

        #endregion Fields

        #region Constructors

        public NotesMemoryRepository()
        {
            _notesTableEntries = new List<NoteTableEntry>
                                 {
                                     new NoteTableEntry("taskList1", "note1"), 
                                     new NoteTableEntry("taskList2", "note2")
                                 };

            _noteOwnerTableEntries = new List<NoteOwnerTableEntry>
                                         {
                                             new NoteOwnerTableEntry("user1", "note1"),
                                             new NoteOwnerTableEntry("user2", "note2")
                                         };

            _noteAssociatedUsersTableEntries = new List<NoteAssociatedUserTableEntry>
                                                   {
                                                       new NoteAssociatedUserTableEntry("note1", "user2"),
                                                       new NoteAssociatedUserTableEntry("note2", "user1")
                                                   };
        }

        #endregion Constructors

        #region Public methods

        public IQueryable<Note> Load()
        {
            return _notesTableEntries.Select(n => n.MapToNote()).AsQueryable();
        }

        public Note Get(string partitionKey, string rowKey)
        {
            var result = _notesTableEntries.FirstOrDefault(n => n.PartitionKey == partitionKey && n.RowKey == rowKey);
            return result != null ? result.MapToNote() : null;
        }

        public void Create(Note entityToCreate)
        {
            var noteTableEntry = entityToCreate.MapToNoteTableEntry();
            var noteOwnerTableEntry = new NoteOwnerTableEntry(entityToCreate.Owner.RowKey, entityToCreate.RowKey);
            var noteAssociatedUserTableEntry = new NoteAssociatedUserTableEntry(entityToCreate.RowKey, entityToCreate.Owner.RowKey);

            _notesTableEntries.Add(noteTableEntry);
            _noteOwnerTableEntries.Add(noteOwnerTableEntry);
            _noteAssociatedUsersTableEntries.Add(noteAssociatedUserTableEntry);
        }

        public void Update(Note entityToUpdate)
        {
            var noteTableEntry = entityToUpdate.MapToNoteTableEntry();
            var noteTableEntryToRemove = _notesTableEntries.First(n => n.PartitionKey == noteTableEntry.PartitionKey && n.RowKey == noteTableEntry.RowKey);

            _notesTableEntries.Remove(noteTableEntryToRemove);
            _notesTableEntries.Add(noteTableEntry);
        }

        public void Delete(Note entityToDelete)
        {
            _notesTableEntries.RemoveAll(n => n.PartitionKey == entityToDelete.PartitionKey && n.RowKey == entityToDelete.RowKey);
            _noteOwnerTableEntries.RemoveAll(n => n.RowKey == entityToDelete.RowKey);
            _noteAssociatedUsersTableEntries.RemoveAll(n => n.PartitionKey == entityToDelete.RowKey);
        }

        public void AddAssociatedUser(Note note, User associatedUser)
        {
            _noteAssociatedUsersTableEntries.Add(new NoteAssociatedUserTableEntry(note.RowKey, associatedUser.RowKey));
        }

        public void RemoveAssociatedUser(Note note, User associatedUser)
        {
            _noteAssociatedUsersTableEntries.RemoveAll(n => n.PartitionKey == note.RowKey && n.RowKey == associatedUser.RowKey);
        }

        #endregion Public methods
    }
}