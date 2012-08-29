using Microsoft.WindowsAzure.StorageClient;

namespace CloudNotes.Repositories.Entities.Relation
{
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