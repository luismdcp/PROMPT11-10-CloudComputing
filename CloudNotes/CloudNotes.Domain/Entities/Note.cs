using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace CloudNotes.Domain.Entities
{
    public class Note : BaseEntity, IValidatableObject
    {
        #region Properties

        public string Title { get; set; }
        public string Content { get; set; }
        public bool IsClosed { get; set; }
        public int OrderingIndex { get; set; }
        public TaskList Container { get; set; }
        public string ContainerKeys { get; set; }
        public User Owner { get; set; }
        public ICollection<User> Share { get; set; }

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