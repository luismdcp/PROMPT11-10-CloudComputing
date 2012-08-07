using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CloudNotes.Domain
{
    public abstract class BaseEntity
    {
        #region Properties

        public DateTime Timestamp { get; set; }
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }

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