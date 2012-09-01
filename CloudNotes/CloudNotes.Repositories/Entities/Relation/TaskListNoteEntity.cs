using Microsoft.WindowsAzure.StorageClient;

namespace CloudNotes.Repositories.Entities.Relation
{
    /// <summary>
    /// Entity related to the TaskListNotes Azure Table.
    /// </summary>
    public class TaskListNoteEntity : TableServiceEntity
    {
        #region Constructors

        public TaskListNoteEntity()
        {

        }

        public TaskListNoteEntity(string partitionKey, string rowKey) : base(partitionKey, rowKey)
        {

        }

        #endregion Constructors
    }
}