using Microsoft.WindowsAzure.StorageClient;

namespace CloudNotes.Repositories.Entities.Relation
{
    public class NoteOwnerTableEntry : TableServiceEntity
    {
        #region Constructors

        public NoteOwnerTableEntry()
        {

        }

        public NoteOwnerTableEntry(string partitionKey, string rowKey) : base(partitionKey, rowKey)
        {

        }

        #endregion Constructors
    }
}