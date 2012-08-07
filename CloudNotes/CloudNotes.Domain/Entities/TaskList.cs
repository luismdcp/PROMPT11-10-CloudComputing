using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace CloudNotes.Domain.Entities
{
    public class TaskList : BaseEntity, IValidatableObject
    {
        #region Properties

        public String Title { get; set; }
        public User Owner { get; set; }
        public IList<Note> Notes { get; private set; }
        public ICollection<User> AssociatedUsers { get; private set; }

        #endregion Properties

        #region Constructors

        public TaskList()
        {
            Notes = new Collection<Note>();
            AssociatedUsers = new Collection<User>();
        }

        public TaskList(string title, User owner) : this()
        {
            Title = title;
            Owner = owner;
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
    }
}