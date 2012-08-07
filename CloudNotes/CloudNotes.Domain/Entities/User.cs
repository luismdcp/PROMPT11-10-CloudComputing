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
        public string Name { get; set; }
        public ICollection<TaskList> OwnedTaskLists { get; private set; }
        public ICollection<Note> OwnedNotes { get; private set; }
        public ICollection<TaskList> AssociatedTaskLists { get; private set; }
        public ICollection<Note> AssociatedNotes { get; private set; }

        #endregion Properties

        #region Constructors

        public User()
        {
            OwnedTaskLists = new Collection<TaskList>();
            OwnedNotes = new Collection<Note>();
            AssociatedTaskLists = new Collection<TaskList>();
            AssociatedNotes = new Collection<Note>();
        }

        public User(string userUniqueIdentifier, string name, string email) : this()
        {
            UserUniqueIdentifier = userUniqueIdentifier;
            Name = name;
            Email = email;
        }

        #endregion Constructors

        #region Validation

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Name) || Name.Length > 10)
            {
                yield return new ValidationResult("Name cannot be empty, all whitespaces or longer than 10 characters.", new[] { "Name" });
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