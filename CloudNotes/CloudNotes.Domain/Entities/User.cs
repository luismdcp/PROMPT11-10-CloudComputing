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

        public string UserUniqueIdentifier { get; set; }
        public string Email { get; set; }
        public ICollection<TaskList> OwnedTaskLists { get; private set; }
        public ICollection<Note> OwnedNotes { get; private set; }
        public ICollection<TaskList> AssociatedTaskLists { get; private set; }
        public ICollection<Note> AssociatedNotes { get; private set; }

        #endregion Properties

        #region Constructors

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

        public User(string partitionKey, string rowKey, string userUniqueIdentifier, string email) : this(partitionKey, rowKey)
        {
            UserUniqueIdentifier = userUniqueIdentifier;
            Email = email;
        }

        #endregion Constructors

        #region Validation

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(UserUniqueIdentifier))
            {
                yield return new ValidationResult("Name cannot be empty or all whitspace.", new[] { "Name" });
            }

            if (!string.IsNullOrWhiteSpace(Email))
            {
                const string emailRegex = @"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*"
                                        + "@"
                                        + @"((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$";

                var re = new Regex(emailRegex);
                var match = re.Match(Email);

                if (!match.Success)
                {
                    yield return new ValidationResult("Email is not valid.", new[] { "Email" });
                }   
            }
        }

        #endregion Validation
    }
}