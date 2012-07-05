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

        #region Constructors

        protected BaseEntity(string partitionKey, string rowKey)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }

        #endregion Constructors

        #region Public methods

        public bool IsValid()
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(this, null, null);
            Validator.TryValidateObject(this, validationContext, validationResults, true);
            return validationResults.Count == 0;
        }

        #endregion

        #region Overrides

        public override bool Equals(object entity)
        {
            return entity != null && entity is BaseEntity && this == (BaseEntity) entity;
        }

        public override int GetHashCode()
        {
            return RowKey.GetHashCode();
        }

        #endregion Overrides

        #region Operators

        public static bool operator ==(BaseEntity entity1, BaseEntity entity2)
        {
            return (object) entity1 == null && (object) entity2 == null || 
                   (object) entity1 != null && (object) entity2 != null &&
                   entity1.RowKey == entity2.RowKey;
        }

        public static bool operator !=(BaseEntity entity1, BaseEntity entity2)
        {
            return !(entity1 == entity2);
        }

        #endregion Operators
    }
}