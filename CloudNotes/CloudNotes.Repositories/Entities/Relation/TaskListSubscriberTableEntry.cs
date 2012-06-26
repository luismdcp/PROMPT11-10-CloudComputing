namespace CloudNotes.Repositories.Entities.Relation
{
    internal class TaskListSubscriberTableEntry : BaseEntity
    {
        #region Constructors

        public TaskListSubscriberTableEntry(string partitionKey, string rowKey) : base(partitionKey, rowKey)
        {
            
        }

        #endregion Constructors
    }
}