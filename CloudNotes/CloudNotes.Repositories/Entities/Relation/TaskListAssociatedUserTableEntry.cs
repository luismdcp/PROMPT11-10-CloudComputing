namespace CloudNotes.Repositories.Entities.Relation
{
    internal class TaskListAssociatedUserTableEntry : BaseEntity
    {
        #region Constructors

        public TaskListAssociatedUserTableEntry(string partitionKey, string rowKey) : base(partitionKey, rowKey)
        {
            
        }

        #endregion Constructors
    }
}