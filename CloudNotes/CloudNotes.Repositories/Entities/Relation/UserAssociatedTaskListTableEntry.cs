namespace CloudNotes.Repositories.Entities.Relation
{
    internal class UserAssociatedTaskListTableEntry : BaseEntity
    {
        #region Constructors

        public UserAssociatedTaskListTableEntry(string partitionKey, string rowKey) : base(partitionKey, rowKey)
        {
            
        }

        #endregion Constructors
    }
}