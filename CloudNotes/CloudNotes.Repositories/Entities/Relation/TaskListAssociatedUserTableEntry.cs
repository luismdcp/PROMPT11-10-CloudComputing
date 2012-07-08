namespace CloudNotes.Repositories.Entities.Relation
{
    public class TaskListAssociatedUserTableEntry : BaseEntity
    {
        #region Constructors

        public TaskListAssociatedUserTableEntry(string partitionKey, string rowKey) : base(partitionKey, rowKey)
        {
            
        }

        #endregion Constructors
    }
}