using Microsoft.WindowsAzure.StorageClient;

namespace CloudNotes.Repositories.Entities
{
    /// <summary>
    /// Entity related to the Notes Azure Table.
    /// </summary>
    public class NoteEntity : TableServiceEntity
    {
        #region Properties

        public string Title { get; set; }
        public string Content { get; set; }
        public bool IsClosed { get; set; }
        public int OrderingIndex { get; set; }
        public string ContainerKeys { get; set; }

        #endregion Properties

        #region Constructors

        public NoteEntity()
        {
            
        }

        public NoteEntity(string partitionKey, string rowKey) : base(partitionKey, rowKey)
        {

        }

        #endregion Constructors
    }
}