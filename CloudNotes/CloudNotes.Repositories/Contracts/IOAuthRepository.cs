using CloudNotes.Repositories.Entities;

namespace CloudNotes.Repositories.Contracts
{
    public interface IOAuthRepository : IRepository<OAuthEntity>
    {
        void CreateToken(TokenEntity token);
        TokenEntity GetToken(string clientId, string tokenType);
    }
}