namespace CloudNotes.Repositories.Entities.Relation
{
    internal class UserSubscribedTaskListTableEntry : BaseEntity
    {
        #region Constructors

        public UserSubscribedTaskListTableEntry(string partitionKey, string rowKey) : base(partitionKey, rowKey)
        {
            
        }

        #endregion Constructors
    }
}