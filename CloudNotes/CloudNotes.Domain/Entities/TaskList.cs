using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace CloudNotes.Domain.Entities
{
    /// <summary>
    /// Class equivalent to a Trello List.
    /// </summary>
    public class TaskList : BaseEntity, IValidatableObject
    {
        #region Properties

        public String Title { get; set; }   // The title or description.
        public User Owner { get; set; } // The User that created the TaskList.
        public IList<Note> Notes { get; private set; }  // List of Notes in the TaskList.
        public ICollection<User> Share { get; private set; }    // List of Users that the TaskList was shared. The TaskList's creator is added to this list on creation.

        #endregion Properties

        #region Constructors

        public TaskList()
        {
            PartitionKey = string.Empty;
            RowKey = string.Empty;
            Notes = new Collection<Note>();
            Share = new Collection<User>();
        }

        public TaskList(string title, User owner) : this()
        {
            Title = title;
            Owner = owner;
        }

        public TaskList(string partitionKey, string rowKey, string title) : this()
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
            Title = title;
        }

        #endregion Constructors

        #region Validation

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Title) || Title.Length > 15)
            {
                yield return new ValidationResult("Title cannot be empty, all whitespaces or longer than 15 characters.", new[] { "Title" });
            }

            if (Owner == null)
            {
                yield return new ValidationResult("A taskList must have an Owner.", new[] { "Owner" });
            }
        }

        #endregion Validation

        #region Public methods

        public override bool Equals(object o)
        {
            if (ReferenceEquals(null, o)) return false;
            if (ReferenceEquals(this, o)) return true;
            if (o.GetType() != typeof(User)) return false;
            return Equals((User) o);
        }

        public bool Equals(User other)
        {
            return !ReferenceEquals(null, other);
        }

        public override int GetHashCode()
        {
            return PartitionKey.GetHashCode() ^ RowKey.GetHashCode();
        }

        #endregion Public methods
    }
}