using Microsoft.WindowsAzure.StorageClient;

namespace CloudNotes.Repositories.Entities
{
    /// <summary>
    /// Entity related to the TaskLists Azure Table.
    /// </summary>
    public class TaskListEntity : TableServiceEntity
    {
        #region Properties

        public string Title { get; set; }

        #endregion Properties

        #region Constructors

        public TaskListEntity()
        {
            
        }

        public TaskListEntity(string partitionKey, string rowKey) : base(partitionKey, rowKey)
        {

        }

        #endregion Constructors
    }
}