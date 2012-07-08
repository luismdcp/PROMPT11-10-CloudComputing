namespace CloudNotes.Repositories.Entities
{
    public class TaskListTableEntry : BaseEntity
    {
        #region Properties

        public string Title { get; set; }

        #endregion Properties

        #region Constructors

        public TaskListTableEntry(string partitionKey, string rowKey) : base(partitionKey, rowKey)
        {
            
        }

        #endregion Constructors
    }
}