using System;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;
using CloudNotes.Domain.Entities;
using CloudNotes.Domain.Services.Contracts;
using CloudNotes.Repositories.Contracts;
using Microsoft.IdentityModel.Claims;
using CloudNotes.Domain.Services.Models;

namespace CloudNotes.Domain.Services.Implementation
{
    public class UsersService : IUsersService
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly IUsersRepository _repository;

        #endregion Fields

        #region Constructors

        public UsersService(IUnitOfWork unitOfWork, IUsersRepository usersRepository)
        {
            _unitOfWork = unitOfWork;
            _repository = usersRepository;
        }

        #endregion Constructors

        #region Public methods

        public IQueryable<User> Load()
        {
            return _repository.Load();
        }

        public User Get(string rowKey)
        {
            var userKeys = rowKey.Split('-');
            var identityProvider = userKeys[1];

            return _repository.Get(u => u.PartitionKey == identityProvider && u.RowKey == rowKey);
        }

        public User Get(string partitionKey, string rowKey)
        {
            return _repository.Get(partitionKey, rowKey);
        }

        public User Get(Expression<Func<User, bool>> filter)
        {
            return _repository.Get(filter);
        }

        public void Create(User entityToCreate)
        {
            _repository.Create(entityToCreate);
            _unitOfWork.SubmitChanges();
        }

        public void Update(User entityToUpdate)
        {
            _repository.Update(entityToUpdate);
            _unitOfWork.SubmitChanges();
        }

        public void Delete(User entityToDelete)
        {
            _repository.Delete(entityToDelete);
            _unitOfWork.SubmitChanges();
        }

        public bool UserIsRegistered(IPrincipal principal)
        {
            return _repository.UserIsRegistered(principal);
        }

        public User GetByIdentifiers(string uniqueIdentifier, string identityProviderIdentifier)
        {
            return _repository.GetByIdentifiers(uniqueIdentifier, identityProviderIdentifier);
        }

        public AuthenticationInfo GetUserAuthenticationInfo(IPrincipal principal)
        {
            var userUniqueIdentifier = string.Empty;
            var userEmailAddress = string.Empty;
            var userName = string.Empty;
            var identityProvider = string.Empty;
            var claimsPrincipal = principal as IClaimsPrincipal;

            if (claimsPrincipal != null)
            {
                var claimsIdentity = claimsPrincipal.Identities[0];
                var nameIdentifierClaim = claimsIdentity.Claims.FirstOrDefault(c => c.ClaimType == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
                var emailAddressClaim = claimsIdentity.Claims.FirstOrDefault(c => c.ClaimType == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");
                var identityProviderClaim = claimsIdentity.Claims.FirstOrDefault(c => c.ClaimType == "http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider");
                var nameClaim = claimsIdentity.Claims.FirstOrDefault(c => c.ClaimType == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name");

                userEmailAddress = emailAddressClaim == null ? string.Empty : emailAddressClaim.Value;
                userName = nameClaim == null ? string.Empty : nameClaim.Value;
                userUniqueIdentifier = nameIdentifierClaim.Value;
                identityProvider = ParseIdentityProvider(identityProviderClaim.Value);
            }

            return new AuthenticationInfo(userName, userEmailAddress, userUniqueIdentifier, identityProvider);
        }

        public void FillAuthenticationInfo(User user, IPrincipal principal)
        {
            string userUniqueIdentifier = string.Empty;
            string identityProviderIdentifier = string.Empty;

            var claimsPrincipal = principal as IClaimsPrincipal;

            if (claimsPrincipal != null)
            {
                var claimsIdentity = claimsPrincipal.Identities[0];
                var nameIdentifierClaim = claimsIdentity.Claims.FirstOrDefault(c => c.ClaimType == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
                var identityProviderClaim = claimsIdentity.Claims.FirstOrDefault(c => c.ClaimType == "http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider");

                userUniqueIdentifier = nameIdentifierClaim.Value;
                identityProviderIdentifier = ParseIdentityProvider(identityProviderClaim.Value);
            }

            user.UniqueIdentifier = userUniqueIdentifier;
            user.PartitionKey = identityProviderIdentifier;
            user.RowKey = string.Format("{0}-{1}", user.Name, identityProviderIdentifier);
        }

        public void LoadOwner(Note note)
        {
            _repository.LoadOwner(note);
        }

        public void LoadOwner(TaskList taskList)
        {
            _repository.LoadOwner(taskList);
        }

        public void LoadShare(Note note)
        {
            _repository.LoadShare(note);
        }

        public void LoadShare(TaskList taskList)
        {
            _repository.LoadShare(taskList);
        }

        public string ParseIdentityProvider(string identityProviderClaim)
        {
            string identityProvider = string.Empty;

            if (identityProviderClaim.Contains("WindowsLiveID"))
            {
                identityProvider = "windowsliveid";
            }
            else
            {
                if (identityProviderClaim.Contains("Google"))
                {
                    identityProvider = "google";
                }
                else
                {
                    if (identityProviderClaim.Contains("Yahoo"))
                    {
                        identityProvider = "yahoo";   
                    }
                }
            }

            return identityProvider;
        }

        #endregion Public methods
    }
}