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
            return _unitOfWork.Load<UserEntity>("Users").ToList().Select(userTableEntry => userTableEntry.MapToUser()).AsQueryable();
        }

        public User Get(string partitionKey, string rowKey)
        {
            var result = _unitOfWork.Get<UserEntity>("Users", partitionKey, rowKey);

            if (result != null)
            {
                return result.MapToUser();
            }

            return null;
        }

        public User Get(Expression<Func<User, bool>> filter)
        {
            return _unitOfWork.Get("Users", filter);
        }

        public void Create(User entityToCreate)
        {
            var userTableEntry = entityToCreate.MapToUserEntity();
            _unitOfWork.Create(userTableEntry, "Users");
        }

        public void Update(User entityToUpdate)
        {
            _unitOfWork.Update("Users", entityToUpdate.MapToUserEntity());
        }

        public void Delete(User entityToDelete)
        {
            _unitOfWork.Delete<UserEntity>("Users", entityToDelete.PartitionKey, entityToDelete.RowKey);
        }

        public bool UserIsRegistered(IPrincipal principal)
        {
            var uniqueIdentifier = string.Empty;
            var identityProvider = string.Empty;
            var claimsPrincipal = principal as IClaimsPrincipal;

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

        public User GetByIdentifiers(string uniqueIdentifier, string identityProviderIdentifier)
        {
            var userEntry = _unitOfWork.Load<UserEntity>("Users", u => u.PartitionKey == identityProviderIdentifier && u.UniqueIdentifier == uniqueIdentifier).FirstOrDefault();
            return userEntry != null ? userEntry.MapToUser() : null;
        }

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

        public void LoadOwner(TaskList taskList)
        {
            var userKeys = taskList.PartitionKey.Split('-');
            var identityProvider = userKeys[1];
            var owner = Get(identityProvider, taskList.PartitionKey);
            taskList.Owner = owner;
        }

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