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

namespace CloudNotes.Tests.Domain.Services.Memory
{
    public class NotesMemoryRepository : INotesRepository
    {
        #region Fields

        public readonly List<NoteEntity> _notes;
        public readonly List<NoteShareEntity> _noteShares;
        public readonly List<TaskListNoteEntity> _taskListNotes;
        public const string IdentityProvider = "windowsliveid";
        public const string User1RowKey = "user1-WindowsLiveID";
        public const string User2RowKey = "user2-WindowsLiveID";
        public const string User3RowKey = "user3-WindowsLiveID";
        public const string Note1PartitionKey = "user1-WindowsLiveID";
        public const string Note2PartitionKey = "user2-WindowsLiveID";
        public readonly string Note1RowKey = ShortGuid.NewGuid().ToString();
        public readonly string Note2RowKey = ShortGuid.NewGuid().ToString();
        public readonly string Note3RowKey = ShortGuid.NewGuid().ToString();
        public const string TaskList1PartitionKey = "user1-WindowsLiveID";
        public const string TaskList2PartitionKey = "user2-WindowsLiveID";
        public readonly string TaskList1RowKey = ShortGuid.NewGuid().ToString();
        public readonly string TaskList2RowKey = ShortGuid.NewGuid().ToString();

        #endregion Fields

        #region Constructors

        public NotesMemoryRepository()
        {
            _notes = new List<NoteEntity>
                        {
                            new NoteEntity(User1RowKey, Note1RowKey) { Title = "Test title" }, 
                            new NoteEntity(User2RowKey, Note2RowKey) { Title = "Another test title" }
                        };

            _noteShares = new List<NoteShareEntity>
                                {
                                    new NoteShareEntity(string.Format("{0}+{1}", Note1PartitionKey, Note1RowKey), User1RowKey),
                                    new NoteShareEntity(string.Format("{0}+{1}", Note1PartitionKey, Note1RowKey), User2RowKey)
                                };

            _taskListNotes = new List<TaskListNoteEntity>
                                 {
                                     new TaskListNoteEntity(string.Format("{0}+{1}", TaskList1PartitionKey, TaskList1RowKey), string.Format("{0}+{1}", Note1PartitionKey, Note1RowKey)),
                                     new TaskListNoteEntity(string.Format("{0}+{1}", TaskList1PartitionKey, TaskList1RowKey), string.Format("{0}+{1}", Note2PartitionKey, Note2RowKey))
                                 };
        }

        #endregion Constructors

        #region Public methods

        public IQueryable<Note> Load()
        {
            return _notes.Select(n => n.MapToNote()).AsQueryable();
        }

        public Note Get(string partitionKey, string rowKey)
        {
            var result = _notes.FirstOrDefault(n => n.PartitionKey == partitionKey && n.RowKey == rowKey);
            return result != null ? result.MapToNote() : null;
        }

        public Note Get(Expression<Func<Note, bool>> filter)
        {
            var result = _notes.Select(n => n.MapToNote()).AsQueryable().FirstOrDefault(filter);
            return result;
        }

        public void Create(Note entityToCreate)
        {
            var note = entityToCreate.MapToNoteEntity();
            var maxIndex = _notes.Max(n => n.OrderingIndex);
            note.OrderingIndex = maxIndex + 1;
            var noteShare = new NoteShareEntity(entityToCreate.RowKey, entityToCreate.Owner.RowKey);

            _notes.Add(note);
            _noteShares.Add(noteShare);
        }

        public void Update(Note entityToUpdate)
        {
            var note = entityToUpdate.MapToNoteEntity();
            var noteToRemove = _notes.First(n => n.PartitionKey == note.PartitionKey && n.RowKey == note.RowKey);

            _notes.Remove(noteToRemove);
            _notes.Add(note);
        }

        public void Delete(Note entityToDelete)
        {
            _notes.RemoveAll(n => n.PartitionKey == entityToDelete.PartitionKey && n.RowKey == entityToDelete.RowKey);
            _noteShares.RemoveAll(n => n.PartitionKey == entityToDelete.RowKey);
        }

        public bool NoteWithTitleExists(string title, TaskList container)
        {
            bool result = false;
            var taskListNotePartitionKey = string.Format("{0}+{1}", container.PartitionKey, container.RowKey);
            var taskListNotes = _taskListNotes.Where(tn => tn.PartitionKey == taskListNotePartitionKey);

            foreach (var taskListNote in taskListNotes)
            {
                var noteKeys = taskListNote.RowKey.Split('+');
                var note = _notes.First(n => n.PartitionKey == noteKeys[0] && n.RowKey == noteKeys[1]);

                if (note.Title == title)
                {
                    result = true;
                    break;
                }
            }

            return result;
        }

        public void AddShare(Note note, string userId)
        {
            _noteShares.Add(new NoteShareEntity(string.Format("{0}+{1}", note.PartitionKey, note.RowKey), userId));
        }

        public void RemoveShare(Note note, string userId)
        {
            _noteShares.RemoveAll(n => n.PartitionKey == string.Format("{0}+{1}", note.PartitionKey, note.RowKey) && n.RowKey == userId);
        }

        public void LoadNotes(TaskList taskList)
        {
            var taskListNotePartitionKey = string.Format("{0}+{1}", taskList.PartitionKey, taskList.RowKey);
            var taskListNotes = _taskListNotes.Where(tn => tn.PartitionKey == taskListNotePartitionKey);

            foreach (var taskListNote in taskListNotes)
            {
                var noteKeys = taskListNote.RowKey.Split('+');
                var note = _notes.First(n => n.PartitionKey == noteKeys[0] && n.RowKey == noteKeys[1]);

                taskList.Notes.Add(note.MapToNote());
            }
        }

        public bool HasPermissionToEdit(User user, Note note)
        {
            var noteSharePartitionKey = string.Format("{0}+{1}", note.PartitionKey, note.RowKey);
            var noteShareRowkey = user.RowKey;

            return _noteShares.Any(ns => ns.PartitionKey == noteSharePartitionKey && ns.RowKey == noteShareRowkey);
        }

        #endregion Public methods
    }
}