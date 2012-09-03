using Microsoft.WindowsAzure.StorageClient;

namespace CloudNotes.Repositories.Entities.OAuth
{
    /// <summary>
    /// Entity related to the OAuthRegisteredApps Azure Table.
    /// </summary>
    public class OAuthEntity : TableServiceEntity
    {
        #region Properties

        public string ClientId { get; set; }
        public string Secret { get; set; }
        public string RedirectUri { get; set; }
        public string Code { get; set; }

        #endregion Properties

        #region Constructors

        public OAuthEntity()
        {
            
        }

        public OAuthEntity(string partitionKey, string rowKey) : base(partitionKey, rowKey)
        {

        }

        public OAuthEntity(string clientId, string secret, string redirectUri, string code) : this()
        {
            ClientId = clientId;
            Secret = secret;
            RedirectUri = redirectUri;
            Code = code;
        }

        #endregion Constructors
    }
}