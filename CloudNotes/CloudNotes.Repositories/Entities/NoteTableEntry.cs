using Microsoft.WindowsAzure.StorageClient;

namespace CloudNotes.Repositories.Entities
{
    public class NoteTableEntry : TableServiceEntity
    {
        #region Properties

        public string Title { get; set; }
        public string Content { get; set; }
        public bool IsClosed { get; set; }
        public int OrderingIndex { get; set; }

        #endregion Properties

        #region Constructors

        public NoteTableEntry()
        {
            
        }

        public NoteTableEntry(string partitionKey, string rowKey) : base(partitionKey, rowKey)
        {

        }

        #endregion Constructors
    }
}