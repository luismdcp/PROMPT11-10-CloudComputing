using System;

namespace CloudNotes.Repositories.Entities
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
    }
}