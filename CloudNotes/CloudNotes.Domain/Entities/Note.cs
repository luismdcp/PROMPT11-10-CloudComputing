using System;
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
        public TaskList ContainerList { get; set; }
        public User Owner { get; set; }
        public ICollection<User> AssociatedUsers { get; private set; }

        #endregion Properties

        #region Constructors

        public Note(string partitionKey, string rowKey) : base(partitionKey, rowKey) 
        {
            if (string.IsNullOrWhiteSpace(partitionKey))
            {
                throw new ArgumentNullException("partitionKey", "A note must have a non-empty PartitionKey.");
            }

            if (string.IsNullOrWhiteSpace(rowKey))
            {
                throw new ArgumentNullException("rowKey", "A note must have a non-empty RowKey.");
            }

            OrderingIndex = -1;
            AssociatedUsers = new Collection<User>();
        }

        public Note(string partitionKey, string rowKey, User owner, TaskList containerList) : this(partitionKey, rowKey)
        {
            Owner = owner;
            ContainerList = containerList;
        }

        #endregion Constructors

        #region Validation

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Title.Length > 20)
            {
                yield return new ValidationResult("Title cannot be longer than 20 characters.", new[] {"Title"});
            }

            if (Content.Length > 50)
            {
                yield return new ValidationResult("Content cannot be longer than 50 characters.", new[] { "Content" });
            }

            if (Owner == null)
            {
                yield return new ValidationResult("A note must have an Owner.", new[] { "Owner" });
            }

            if (ContainerList == null)
            {
                yield return new ValidationResult("A note must be inside a list.", new[] { "ContainerList" });
            }
        }

        #endregion Validation
    }
}