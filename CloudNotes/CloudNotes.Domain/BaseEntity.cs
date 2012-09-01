using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CloudNotes.Domain
{
    public abstract class BaseEntity
    {
        #region Properties

        public DateTime Timestamp { get; set; } // Readonly Timestamp automatically filled by the Azure Tables Context.
        public string PartitionKey { get; set; } // Partition key of the table entity.
        public string RowKey { get; set; }  // Row key of the table entity.

        #endregion Properties

        #region Public methods

        public bool IsValid()
        {
            return GetValidationErrors().Count == 0;
        }

        public List<ValidationResult> GetValidationErrors()
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(this, null, null);
            Validator.TryValidateObject(this, validationContext, validationResults, true);
            return validationResults;
        }

        #endregion
    }
}