using Microsoft.WindowsAzure.StorageClient;

namespace CloudNotes.Repositories.Entities.Relation
{
    public class TaskListShareEntity : TableServiceEntity
    {
        #region Constructors

        public TaskListShareEntity()
        {

        }

        public TaskListShareEntity(string partitionKey, string rowKey) : base(partitionKey, rowKey)
        {

        }

        #endregion Constructors
    }
}