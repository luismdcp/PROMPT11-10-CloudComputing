using Microsoft.WindowsAzure.StorageClient;

namespace CloudNotes.Repositories.Entities.Relation
{
    public class NoteAssociatedUserTableEntry : TableServiceEntity
    {
        #region Constructors

        public NoteAssociatedUserTableEntry()
        {

        }

        public NoteAssociatedUserTableEntry(string partitionKey, string rowKey) : base(partitionKey, rowKey)
        {

        }

        #endregion Constructors
    }
}