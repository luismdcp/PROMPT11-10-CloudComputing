using Microsoft.WindowsAzure.StorageClient;

namespace CloudNotes.Repositories.Entities
{
    public class UserTableEntry : TableServiceEntity
    {
        #region Properties

        public string Email { get; set; }
        public string Name { get; set; }

        #endregion Properties

        #region Constructors

        public UserTableEntry()
        {
            
        }

        public UserTableEntry(string partitionKey, string rowKey) : base(partitionKey, rowKey)
        {

        }

        #endregion Constructors
    }
}