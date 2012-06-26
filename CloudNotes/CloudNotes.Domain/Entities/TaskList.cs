using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace CloudNotes.Domain.Entities
{
    public class TaskList : BaseEntity, IValidatableObject
    {
        #region Properties

        public string Title { get; set; }
        public User Owner { get; set; }
        public IList<Note> Notes { get; private set; }
        public ICollection<User> AssociatedUsers { get; private set; }

        #endregion Properties

        #region Constructors

        public TaskList(string partitionKey, string rowKey) : base(partitionKey, rowKey)
        {
            if (string.IsNullOrWhiteSpace(partitionKey))
            {
                throw new ArgumentNullException("partitionKey", "A taskList must have a non-empty PartitionKey.");
            }

            if (string.IsNullOrWhiteSpace(rowKey))
            {
                throw new ArgumentNullException("rowKey", "A taskList must have a non-empty RowKey.");
            }

            Notes = new Collection<Note>();
            AssociatedUsers = new Collection<User>();
        }

        public TaskList(string partitionKey, string rowKey, User owner) : this(partitionKey, rowKey)
        {
            Owner = owner;
        }

        #endregion Constructors

        #region Validation

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Title.Length > 20)
            {
                yield return new ValidationResult("Title cannot be longer than 20 characters.", new[] { "Title" });
            }

            if (Owner == null)
            {
                yield return new ValidationResult("A taskList must have an Owner.", new[] { "Owner" });
            }
        }

        #endregion Validation

        #region Public methods

        public void AddNote(Note newNote)
        {
            newNote.OrderingIndex = Notes.Count - 1;
            Notes.Add(newNote);
            newNote.ContainerList = this;
        }

        public void MoveUp(Note note)
        {
            int index = Notes.IndexOf(note);
            Note previousNote = Notes[index - 1];

            if (index == -1)
            {
                throw new ArgumentOutOfRangeException("note");
            }

            if (index == 0)
            {
                return;
            }

            Swap(Notes, index, index - 1);
            note.OrderingIndex = index - 1;
            previousNote.OrderingIndex = index;
        }

        public void MoveDown(Note note)
        {
            int index = Notes.IndexOf(note);
            Note nextNote = Notes[index + 1];

            if (index == -1)
            {
                throw new ArgumentOutOfRangeException("note");
            }

            if (index == 0)
            {
                return;
            }

            Swap(Notes, index, index + 1);
            note.OrderingIndex = index + 1;
            nextNote.OrderingIndex = index;
        }

        #endregion Public methods

        #region Private methods

        private static void Swap(IList<Note> notesList, int index1, int index2)
        {
            Note temp = notesList[index1];
            notesList[index1] = notesList[index2];
            notesList[index2] = temp;
        }

        #endregion Private methods
    }
}