using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace CloudNotes.Domain.Entities
{
    public class User : BaseEntity, IValidatableObject
    {
        #region Properties

        public string UniqueIdentifier { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public ICollection<TaskList> OwnedTaskLists { get; private set; }
        public ICollection<Note> OwnedNotes { get; private set; }
        public ICollection<TaskList> SharedTaskLists { get; private set; }
        public ICollection<Note> SharedNotes { get; private set; }

        #endregion Properties

        #region Constructors

        public User()
        {
            PartitionKey = string.Empty;
            RowKey = string.Empty;
            OwnedTaskLists = new Collection<TaskList>();
            OwnedNotes = new Collection<Note>();
            SharedTaskLists = new Collection<TaskList>();
            SharedNotes = new Collection<Note>();
        }

        public User(string userUniqueIdentifier, string name, string email) : this()
        {
            UniqueIdentifier = userUniqueIdentifier;
            Name = name;
            Email = email;
        }

        #endregion Constructors

        #region Validation

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Name.Contains("-") || Name.Contains("+"))
            {
                yield return new ValidationResult("Name cannot have the characters '-' or '+'.", new[] { "Title" });
            }

            if (string.IsNullOrWhiteSpace(Name) || Name.Length > 15)
            {
                yield return new ValidationResult("Name cannot be empty, all whitespaces or longer than 15 characters.", new[] { "Name" });
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

        #region Public methods

        public override bool Equals(object o)
        {
            if (ReferenceEquals(null, o)) return false;
            if (ReferenceEquals(this, o)) return true;
            if (o.GetType() != typeof (User)) return false;
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