using Microsoft.WindowsAzure.StorageClient;

namespace CloudNotes.Repositories.Entities
{
    /// <summary>
    /// Entity related to the Users Azure Table.
    /// </summary>
    public class UserEntity : TableServiceEntity
    {
        #region Properties

        public string Email { get; set; }
        public string Name { get; set; }
        public string UniqueIdentifier { get; set; }    // Unique identifier provided by the Identity Provider

        #endregion Properties

        #region Constructors

        public UserEntity()
        {
            
        }

        public UserEntity(string partitionKey, string rowKey) : base(partitionKey, rowKey)
        {

        }

        #endregion Constructors
    }
}