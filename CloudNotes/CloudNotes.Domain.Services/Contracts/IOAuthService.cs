using CloudNotes.Repositories.Entities;

namespace CloudNotes.Domain.Services.Contracts
{
    public interface IOAuthService : IService<OAuthEntity>
    {
        void CreateToken(TokenEntity token);
        TokenEntity GetToken(string clientId, string tokenType);
    }
}