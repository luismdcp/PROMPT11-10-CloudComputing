using Microsoft.WindowsAzure.StorageClient;

namespace CloudNotes.Repositories.Entities.Relation
{
    /// <summary>
    /// Entity related to the NoteShares Azure Table.
    /// </summary>
    public class NoteShareEntity : TableServiceEntity
    {
        #region Constructors

        public NoteShareEntity()
        {

        }

        public NoteShareEntity(string partitionKey, string rowKey) : base(partitionKey, rowKey)
        {

        }

        #endregion Constructors
    }
}