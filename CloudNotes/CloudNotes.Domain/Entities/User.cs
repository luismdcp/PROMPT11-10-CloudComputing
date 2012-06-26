using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace CloudNotes.Domain.Entities
{
    public class User : BaseEntity, IValidatableObject
    {
        #region Properties

        public string Name { get; set; }
        public string Email { get; set; }
        public ICollection<TaskList> OwnedTaskLists { get; private set; }
        public ICollection<Note> OwnedNotes { get; private set; }
        public ICollection<TaskList> AssociatedTaskLists { get; private set; }
        public ICollection<Note> AssociatedNotes { get; private set; }

        #endregion Properties

        #region Constructor

        public User(string partitionKey, string rowKey) : base(partitionKey, rowKey)
        {
            if (string.IsNullOrWhiteSpace(partitionKey))
            {
                throw new ArgumentNullException("partitionKey", "A user must have a non-empty PartitionKey.");
            }

            if (string.IsNullOrWhiteSpace(rowKey))
            {
                throw new ArgumentNullException("rowKey", "A user must have a non-empty RowKey.");
            }

            OwnedTaskLists = new Collection<TaskList>();
            OwnedNotes = new Collection<Note>();
            AssociatedTaskLists = new Collection<TaskList>();
            AssociatedNotes = new Collection<Note>();
        }

        #endregion Constructor

        #region Validation

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Name.Length > 10)
            {
                yield return new ValidationResult("Name cannot be longer than 10 characters.", new[] { "Name" });
            }

            const string emailRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                                      @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                                      @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";

            var re = new Regex(emailRegex);

            if (!re.IsMatch(emailRegex))
            {
                yield return new ValidationResult("Email is not valid.", new[] { "Email" });
            }
        }

        #endregion Validation
    }
}