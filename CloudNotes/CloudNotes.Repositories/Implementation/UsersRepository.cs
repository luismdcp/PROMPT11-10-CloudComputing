using System;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;
using CloudNotes.Domain.Entities;
using CloudNotes.Repositories.Contracts;
using CloudNotes.Repositories.Entities;
using CloudNotes.Repositories.Entities.Relation;
using CloudNotes.Repositories.Extensions;
using Microsoft.IdentityModel.Claims;

namespace CloudNotes.Repositories.Implementation
{

    /// <summary>
    /// Repository to manage all the actions related to the Users Azure Table.
    /// </summary>
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

        /// <summary>
        /// Loads all the entities in the Users Azure Table.
        /// </summary>
        /// <returns>IQueryable of all the User entities</returns>
        public IQueryable<User> Load()
        {
            return _unitOfWork.Load<UserEntity>("Users").ToList().Select(userTableEntry => userTableEntry.MapToUser()).AsQueryable();
        }

        /// <summary>
        /// Gets a single entity from the Users Azure Table and maps it to a User domain object.
        /// </summary>
        /// <param name="partitionKey">The User partitionkey</param>
        /// <param name="rowKey">The User rowkey</param>
        /// <returns>A User if exists, null otherwise</returns>
        public User Get(string partitionKey, string rowKey)
        {
            var result = _unitOfWork.Get<UserEntity>("Users", partitionKey, rowKey);

            if (result != null)
            {
                return result.MapToUser();
            }

            return null;
        }

        /// <summary>
        /// Gets a single entity from the Users Azure Table by filter and maps it to a User domain object.
        /// </summary>
        /// <param name="filter">Lambda to filter the User</param>
        /// <returns>A User if exists, null otherwise</returns>
        public User Get(Expression<Func<User, bool>> filter)
        {
            return _unitOfWork.Get("Users", filter);
        }

        /// <summary>
        /// Creates a new User.
        /// </summary>
        /// <param name="entityToCreate">User domain object with the properties to map to an Users table entity</param>
        public void Create(User entityToCreate)
        {
            var userTableEntry = entityToCreate.MapToUserEntity();
            _unitOfWork.Create(userTableEntry, "Users");
        }

        /// <summary>
        /// Updates an entity in the Users Azure Table.
        /// </summary>
        ///<param name="entityToUpdate">User domain object with the properties to update an existing Users table entity</param>
        public void Update(User entityToUpdate)
        {
            _unitOfWork.Update("Users", entityToUpdate.MapToUserEntity());
        }

        /// <summary>
        /// Deletes an entity from the Users Azure Table.
        /// </summary>
        /// <param name="entityToDelete">User domain object with the properties to delete an existing Users table entity</param>
        public void Delete(User entityToDelete)
        {
            _unitOfWork.Delete<UserEntity>("Users", entityToDelete.PartitionKey, entityToDelete.RowKey);
        }

        /// <summary>
        /// Checks if an authenticated user is already registered in the User's Azure table.
        /// </summary>
        /// <param name="principal">Object wiht the authentication claims for the logged user</param>
        /// <returns>True if the logged user already exists in the Users Azure Table, False otherwise</returns>
        public bool UserIsRegistered(IPrincipal principal)
        {
            var uniqueIdentifier = string.Empty;
            var identityProvider = string.Empty;
            var claimsPrincipal = principal as IClaimsPrincipal;

            // If the user was authenticated with the Windows Identity Foundation through the Azure Acess Control System.
            if (claimsPrincipal != null)
            {
                var claimsIdentity = claimsPrincipal.Identities[0];
                var nameIdentifierClaim = claimsIdentity.Claims.FirstOrDefault(c => c.ClaimType == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
                var identityProviderClaim = claimsIdentity.Claims.FirstOrDefault(c => c.ClaimType == "http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider");

                identityProvider = ParseIdentityProvider(identityProviderClaim.Value);
                uniqueIdentifier = nameIdentifierClaim.Value;
            }

            var existingUserEntry = _unitOfWork.Load<UserEntity>("Users", u => u.PartitionKey == identityProvider && u.UniqueIdentifier == uniqueIdentifier).FirstOrDefault();
            return existingUserEntry != null;
        }

        /// <summary>
        /// Gets a User by the UniqueIdentifer and IdentityProvider claims values.
        /// </summary>
        /// <param name="uniqueIdentifier">UniqueIdentifier claim value</param>
        /// <param name="identityProviderIdentifier">IdentityProvider claim value</param>
        /// <returns>The User with those properties from the Users Azure Table</returns>
        public User GetByIdentifiers(string uniqueIdentifier, string identityProviderIdentifier)
        {
            var userEntry = _unitOfWork.Load<UserEntity>("Users", u => u.PartitionKey == identityProviderIdentifier && u.UniqueIdentifier == uniqueIdentifier).FirstOrDefault();
            return userEntry != null ? userEntry.MapToUser() : null;
        }

        /// <summary>
        /// Fills the Note Owner information with an User object.
        /// </summary>
        /// <param name="note">Note to have the Owner filled.</param>
        public void LoadOwner(Note note)
        {
            var ownerKeys = note.PartitionKey.Split('-');
            var ownerPartitionKey = ownerKeys[1];
            var owner = Get(ownerPartitionKey, note.PartitionKey);

            if (owner != null)
            {
                note.Owner = owner;
            }
        }

        /// <summary>
        /// Fills the note share users list.
        /// </summary>
        /// <param name="note">Note to have the Share list filled</param>
        public void LoadShare(Note note)
        {
            var noteSharePartitionKey = string.Format("{0}+{1}", note.PartitionKey, note.RowKey);
            var noteShares = _unitOfWork.Load<NoteShareEntity>("NoteShares", n => n.PartitionKey == noteSharePartitionKey);

            foreach (var noteShare in noteShares)
            {
                var userKeys = noteShare.RowKey.Split('-');
                var identityProvider = userKeys[1];

                var share = Get(identityProvider, noteShare.RowKey);

                if (share != null)
                {
                    note.Share.Add(share);
                }
            }
        }

        /// <summary>
        /// Fills the TaskList Owner information wiht an User object.
        /// </summary>
        /// <param name="taskList">TaskList to have the Owner filled.</param>
        public void LoadOwner(TaskList taskList)
        {
            var userKeys = taskList.PartitionKey.Split('-');
            var identityProvider = userKeys[1];
            var owner = Get(identityProvider, taskList.PartitionKey);
            taskList.Owner = owner;
        }

        /// <summary>
        /// Fills the tasklist share users list.
        /// </summary>
        /// <param name="taskList">TaskList to have the Share list filled</param>
        public void LoadShare(TaskList taskList)
        {
            var taskListSharePartitionKey = string.Format("{0}+{1}", taskList.PartitionKey, taskList.RowKey);
            var taskListShares = _unitOfWork.Load<TaskListShareEntity>("TaskListShares", n => n.PartitionKey == taskListSharePartitionKey);

            foreach (var taskListShare in taskListShares)
            {
                var userKeys = taskListShare.RowKey.Split('-');
                var identityProvider = userKeys[1];

                var share = Get(identityProvider, taskListShare.RowKey);

                if (share != null)
                {
                    taskList.Share.Add(share);
                }
            }
        }

        #endregion Public methods

        #region Static methods

        /// <summary>
        /// Parses the Identity Provider identifier from the Indentity Provider claim.
        /// </summary>
        /// <param name="identityProviderClaim">Identity Provider claim value</param>
        /// <returns>The identity provider simple name</returns>
        public static string ParseIdentityProvider(string identityProviderClaim)
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

        #endregion Static methods
    }
}