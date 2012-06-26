namespace CloudNotes.Repositories.Entities
{
    internal class UserTableEntry : BaseEntity
    {
        #region Properties

        public string Email { get; set; }

        #endregion Properties

        #region Constructors

        public UserTableEntry(string partitionKey, string rowKey) : base(partitionKey, rowKey)
        {
            
        }

        #endregion Constructors
    }
}