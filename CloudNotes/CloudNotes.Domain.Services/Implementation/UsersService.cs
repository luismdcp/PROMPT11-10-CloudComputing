using System;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;
using CloudNotes.Domain.Entities;
using CloudNotes.Domain.Services.Contracts;
using CloudNotes.Domain.Services.Models;
using CloudNotes.Repositories.Contracts;
using Microsoft.IdentityModel.Claims;

namespace CloudNotes.Domain.Services.Implementation
{
    /// <summary>
    /// Service class to manage all the operations related to Users.
    /// </summary>
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

        /// <summary>
        /// Load all Users.
        /// </summary>
        /// <returns>IQueryable of all Users</returns>
        public IQueryable<User> Load()
        {
            return _repository.Load();
        }

        /// <summary>
        /// Get a User by his row key.
        /// </summary>
        /// <param name="rowKey">The User row key</param>
        /// <returns>A User if exists, null otherwise</returns>
        public User Get(string rowKey)
        {
            var userKeys = rowKey.Split('-');
            var identityProvider = userKeys[1];

            return _repository.Get(u => u.PartitionKey == identityProvider && u.RowKey == rowKey);
        }

        /// <summary>
        /// Get a User by his partiton key and row key.
        /// </summary>
        /// <param name="partitionKey">The User partition key</param>
        /// <param name="rowKey">The User row key</param>
        /// <returns>A User if exists, null otherwise</returns>
        public User Get(string partitionKey, string rowKey)
        {
            return _repository.Get(partitionKey, rowKey);
        }

        /// <summary>
        /// Get a User by a filter.
        /// </summary>
        /// <param name="filter">Lambda expression filter</param>
        /// <returns>A User if exists, null otherwise</returns>
        public User Get(Expression<Func<User, bool>> filter)
        {
            return _repository.Get(filter);
        }

        /// <summary>
        /// Create a User.
        /// </summary>
        /// <param name="entityToCreate">User to create</param>
        public void Create(User entityToCreate)
        {
            _repository.Create(entityToCreate);
            _unitOfWork.SubmitChanges();
        }

        /// <summary>
        /// Update a User.
        /// </summary>
        /// <param name="entityToUpdate">User to update</param>
        public void Update(User entityToUpdate)
        {
            _repository.Update(entityToUpdate);
            _unitOfWork.SubmitChanges();
        }

        /// <summary>
        /// Delete a User.
        /// </summary>
        /// <param name="entityToDelete">User to delete</param>
        public void Delete(User entityToDelete)
        {
            _repository.Delete(entityToDelete);
            _unitOfWork.SubmitChanges();
        }

        /// <summary>
        /// Check if a User is registered at the application.
        /// </summary>
        /// <param name="principal">Authenticated user IPrincipal object</param>
        /// <returns>True if the User is already registered, False otherwise</returns>
        public bool UserIsRegistered(IPrincipal principal)
        {
            return _repository.UserIsRegistered(principal);
        }

        /// <summary>
        /// Get a User by the unique identifier provided by the identiy provider and by the identity provider identitifer (name).
        /// </summary>
        /// <param name="uniqueIdentifier">Unique identifier provided by the identiy provider</param>
        /// <param name="identityProviderIdentifier">Identity provider identifier (name)</param>
        /// <returns>The User with the unique identifer and from the identity provider</returns>
        public User GetByIdentifiers(string uniqueIdentifier, string identityProviderIdentifier)
        {
            return _repository.GetByIdentifiers(uniqueIdentifier, identityProviderIdentifier);
        }

        /// <summary>
        /// Extract the claims values from the authenticated User IPrincipal object.
        /// </summary>
        /// <param name="principal">Authenticated user IPrincipal object</param>
        /// <returns>Object with the authenticated User extracted from the claims</returns>
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

        /// <summary>
        /// Fill an User object with the unique identifier, partition key and row key.
        /// </summary>
        /// <param name="user">User to fill in the data</param>
        /// <param name="principal">Authenticated user IPrincipal object</param>
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

        /// <summary>
        /// Load the User that created the Note.
        /// </summary>
        /// <param name="note">Note to fill the Owner data</param>
        public void LoadOwner(Note note)
        {
            _repository.LoadOwner(note);
        }

        /// <summary>
        /// Load the User that created the TaskList.
        /// </summary>
        /// <param name="taskList">TaskList to fill the Owner data</param>
        public void LoadOwner(TaskList taskList)
        {
            _repository.LoadOwner(taskList);
        }

        /// <summary>
        /// Load all the Users that the Note was shared with.
        /// </summary>
        /// <param name="note">Note to fill the Share list</param>
        public void LoadShare(Note note)
        {
            _repository.LoadShare(note);
        }

        /// <summary>
        /// Load all the Users that the TaskList was shared with.
        /// </summary>
        /// <param name="taskList">TaskList to fill the Share list</param>
        public void LoadShare(TaskList taskList)
        {
            _repository.LoadShare(taskList);
        }

        /// <summary>
        /// Extract a friendly name from the Identity Provider claim.
        /// </summary>
        /// <param name="identityProviderClaim">Identity Provider claim</param>
        /// <returns>Friendly name for the Identity Provider</returns>
        public string ParseIdentityProvider(string identityProviderClaim)
        {
            var identityProvider = string.Empty;

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