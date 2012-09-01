using Microsoft.WindowsAzure.StorageClient;

namespace CloudNotes.Repositories.Entities
{
    /// <summary>
    /// Entity related to the OAuthTokens Azure Table.
    /// </summary>
    public class TokenEntity : TableServiceEntity
    {
        #region Properties

        public string ClientId { get; set; }
        public string AcessToken { get; set; }
        public string TokenType { get; set; }
        public double ExpiresIn { get; set; }

        #endregion Properties

        #region Constructors

        public TokenEntity()
        {
            
        }

        public TokenEntity(string partitionKey, string rowKey) : base(partitionKey, rowKey)
        {

        }

        public TokenEntity(string clientId, string accessToken, string tokenType, double expiresIn) : this()
        {
            ClientId = clientId;
            AcessToken = accessToken;
            TokenType = tokenType;
            ExpiresIn = expiresIn;
        }

        #endregion Constructors
    }
}