﻿namespace CloudNotes.Repositories.Entities
{
    public class UserTableEntry : BaseEntity
    {
        #region Properties

        public string Email { get; set; }

        #endregion Properties

        #region Constructors

        public UserTableEntry()
        {
            
        }

        public UserTableEntry(string partitionKey, string rowKey) : base(partitionKey, rowKey)
        {
            
        }

        #endregion Constructors
    }
}