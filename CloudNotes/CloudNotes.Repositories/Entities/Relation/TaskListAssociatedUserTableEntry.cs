using Microsoft.WindowsAzure.StorageClient;

namespace CloudNotes.Repositories.Entities.Relation
{
    public class TaskListAssociatedUserTableEntry : TableServiceEntity
    {
        #region Constructors

        public TaskListAssociatedUserTableEntry()
        {

        }

        public TaskListAssociatedUserTableEntry(string partitionKey, string rowKey) : base(partitionKey, rowKey)
        {

        }

        #endregion Constructors
    }
}