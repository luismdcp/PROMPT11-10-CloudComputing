namespace CloudNotes.Repositories.Entities.Relation
{
    internal class UserOwnedTaskListTableEntry : BaseEntity
    {
        #region Constructors

        public UserOwnedTaskListTableEntry(string partitionKey, string rowKey) : base(partitionKey, rowKey)
        {
            
        }

        #endregion Constructors
    }
}