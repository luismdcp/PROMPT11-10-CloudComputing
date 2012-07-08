using System.Linq;
using System.Threading;
using CloudNotes.Domain.Entities;
using CloudNotes.Repositories.Contracts;
using CloudNotes.Repositories.Entities;
using CloudNotes.Repositories.Extensions;
using Microsoft.IdentityModel.Claims;

namespace CloudNotes.Repositories.Implementation
{
    public class UsersRepository : IUsersRepository
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;

        #endregion Fields

        #region Constructors

        public UsersRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Constructors

        #region Public methods

        public IQueryable<User> Load()
        {
            var usersTableEntries = _unitOfWork.Load<UserTableEntry>("Users");
            return usersTableEntries.Select(userTableEntry => userTableEntry.MapToUser()).AsQueryable();
        }

        public User Get(string partitionKey, string rowKey)
        {
            return _unitOfWork.Get<UserTableEntry>("Users", partitionKey, rowKey).MapToUser();
        }

        public void Create(User entityToAdd)
        {
            var userTableEntry = entityToAdd.MapToUserTableEntry();
            _unitOfWork.Add(userTableEntry, "Users");
        }

        public void Update(User entityToUpdate)
        {
            var userTableEntry = entityToUpdate.MapToUserTableEntry();
            _unitOfWork.Update(userTableEntry);
        }

        public void Delete(User entityToDelete)
        {
            var userTableEntry = entityToDelete.MapToUserTableEntry();
            _unitOfWork.Delete(userTableEntry);
        }

        public User GetOrAddCurrentUser()
        {
            string userUniqueIdentifier;
            string userEmailAddress = string.Empty;
            var claimsPrincipal = Thread.CurrentPrincipal as IClaimsPrincipal;

            if (claimsPrincipal != null)
            {
                IClaimsIdentity claimsIdentity = claimsPrincipal.Identities[0];
                Claim nameIdentifierClaim = claimsIdentity.Claims.FirstOrDefault(claim => claim.ClaimType == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
                Claim emailAddressClaim = claimsIdentity.Claims.FirstOrDefault(claim => claim.ClaimType == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");
                Claim identityProviderClaim = claimsIdentity.Claims.FirstOrDefault(claim => claim.ClaimType == "http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider");

                userUniqueIdentifier = nameIdentifierClaim.Value + identityProviderClaim.Value;
                userEmailAddress = emailAddressClaim == null ? string.Empty : emailAddressClaim.Value;
            }
            else
            {
                userUniqueIdentifier = Thread.CurrentPrincipal.Identity.Name;
            }

            var newOrExitingUserEntry = _unitOfWork.Load<UserTableEntry>("Users").Where(u => u.RowKey == userUniqueIdentifier).FirstOrDefault();

            if (newOrExitingUserEntry == null)
            {
                var user = new User("users", userUniqueIdentifier, userUniqueIdentifier, userEmailAddress);
                Create(user);
                return user;
            }

            return newOrExitingUserEntry.MapToUser();
        }

        #endregion Public methods
    }
}