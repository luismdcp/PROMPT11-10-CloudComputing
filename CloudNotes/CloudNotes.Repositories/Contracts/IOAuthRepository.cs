using CloudNotes.Repositories.Entities;
using CloudNotes.Repositories.Entities.OAuth;

namespace CloudNotes.Repositories.Contracts
{
    public interface IOAuthRepository : IRepository<OAuthEntity>
    {
        void CreateToken(TokenEntity token);
        TokenEntity GetToken(string clientId, string tokenType);
    }
}