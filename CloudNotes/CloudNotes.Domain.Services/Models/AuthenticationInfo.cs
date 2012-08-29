
namespace CloudNotes.Domain.Services.Models
{
    public class AuthenticationInfo
    {
        #region Properties

        public string Name { get; set; }
        public string Email { get; set; }
        public string UniqueIdentifier { get; set; }
        public string IdentityProviderIdentifier { get; set; }

        #endregion Properties

        #region Constructors

        public AuthenticationInfo(string name, string email, string uniqueIdentifier, string indentityProviderIdentifier)
        {
            Name = name;
            Email = email;
            UniqueIdentifier = uniqueIdentifier;
            IdentityProviderIdentifier = indentityProviderIdentifier;
        }

        #endregion Constructors
    }
}