using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace CloudNotes.Domain.Entities
{
    /// <summary>
    /// Class equivalent to a Trello Card.
    /// </summary>
    public class Note : BaseEntity, IValidatableObject
    {
        #region Properties

        public string Title { get; set; }   // The title or description.
        public string Content { get; set; } // The content.
        public bool IsClosed { get; set; }  // If the Note is closed or not.
        public int OrderingIndex { get; set; } // Order index related to the Note's position in the TaskList Notes list.
        public TaskList Container { get; set; } // The TaskList where the Note is contained.
        public string ContainerKeys { get; set; }   // Combination of the PartitionKey and RowKey of the TaskList where the Note is contained.
        public User Owner { get; set; } // The User that created the Note.
        public ICollection<User> Share { get; set; }    // List of Users that the Note was shared. The Note's creator is added to this list on creation.

        #endregion Properties

        #region Constructors

        public Note()
        {
            PartitionKey = string.Empty;
            RowKey = string.Empty;
            Title = string.Empty;
            Content = string.Empty;
            OrderingIndex = -1;
            Share = new Collection<User>();
        }

        public Note(string partitionKey, string rowKey, string title, string content, bool isCLosed) : this()
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
            Title = title;
            Content = content;
            IsClosed = isCLosed;
        }

        public Note(string title, string content, User owner, TaskList containerList) : this()
        {
            Title = title;
            Content = content;
            Owner = owner;
            Container = containerList;
        }

        #endregion Constructors

        #region Validation

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Title) || Title.Length > 10)
            {
                yield return new ValidationResult("Title cannot be empty, all whitespaces or longer than 10 characters.", new[] { "Title" });
            }

            if (string.IsNullOrWhiteSpace(Content) || Content.Length > 30)
            {
                yield return new ValidationResult("Content cannot be empty, all whitespaces or longer than 30 characters.", new[] { "Content" });
            }

            if (Owner == null)
            {
                yield return new ValidationResult("A note must have an Owner.", new[] { "Owner" });
            }

            if (Container == null)
            {
                yield return new ValidationResult("A note must be inside a list.", new[] { "ContainerList" });
            }
        }

        #endregion Validation
    }
}