using System;
using System.Data.Services.Common;

namespace CloudNotes.Repositories.Entities
{
    [DataServiceKey(new[] { "PartitionKey", "RowKey" })]
    public abstract class BaseEntity
    {
        #region Properties

        public DateTime Timestamp { get; set; }
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }

        #endregion Properties

        #region Constructors

        protected BaseEntity()
        {
            
        }

        protected BaseEntity(string partitionKey, string rowKey)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }

        #endregion Constructors
    }
}