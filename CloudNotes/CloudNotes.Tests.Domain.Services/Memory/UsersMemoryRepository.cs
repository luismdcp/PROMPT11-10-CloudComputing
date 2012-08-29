using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;
using CloudNotes.Domain.Entities;
using CloudNotes.Repositories.Contracts;
using CloudNotes.Repositories.Entities;
using CloudNotes.Repositories.Entities.Relation;
using CloudNotes.Repositories.Extensions;
using CloudNotes.Repositories.Helpers;
using Microsoft.IdentityModel.Claims;

namespace CloudNotes.Tests.Domain.Services.Memory
{
    public class UsersMemoryRepository : IUsersRepository
    {
        #region Fields

        public readonly List<UserEntity> Users;
        public readonly List<NoteShareEntity> NoteShares;
        public readonly List<TaskListShareEntity> TaskListShares;
        public const string IdentityProvider = "windowsliveid";
        public const string User1RowKey = "user1-windowsliveid";
        public const string User2RowKey = "user2-windowsliveid";
        public const string User3RowKey = "user3-windowsliveid";
        public const string User4RowKey = "user4-windowsliveid";
        public const string Note1PartitionKey = "user1-windowsliveid";
        public const string Note2PartitionKey = "user2-windowsliveid";
        public readonly string Note1RowKey = ShortGuid.NewGuid().ToString();
        public readonly string Note2RowKey = ShortGuid.NewGuid().ToString();
        public readonly string Note3RowKey = ShortGuid.NewGuid().ToString();
        public const string TaskList1PartitionKey = "user1-windowsliveid";
        public const string TaskList2PartitionKey = "user2-windowsliveid";
        public readonly string TaskList1RowKey = ShortGuid.NewGuid().ToString();
        public readonly string TaskList2RowKey = ShortGuid.NewGuid().ToString();
        public readonly string TaskList3RowKey = ShortGuid.NewGuid().ToString();

        #endregion Fields

        #region Constructors

        public UsersMemoryRepository()
        {
            Users = new List<UserEntity>
                        {
                            new UserEntity(IdentityProvider, User1RowKey) { UniqueIdentifier = "user1" }, 
                            new UserEntity(IdentityProvider, User2RowKey),
                            new UserEntity(IdentityProvider, User3RowKey)
                        };


            NoteShares = new List<NoteShareEntity>
                            {
                                new NoteShareEntity(string.Format("{0}+{1}", Note1PartitionKey, Note1RowKey), User1RowKey),
                                new NoteShareEntity(string.Format("{0}+{1}", Note1PartitionKey, Note1RowKey), User2RowKey)
                            };

            TaskListShares = new List<TaskListShareEntity>
                                {
                                    new TaskListShareEntity(string.Format("{0}+{1}", TaskList1PartitionKey, TaskList1RowKey), User1RowKey),
                                    new TaskListShareEntity(string.Format("{0}+{1}", TaskList1PartitionKey, TaskList1RowKey), User3RowKey),
                                    new TaskListShareEntity(string.Format("{0}+{1}", TaskList2PartitionKey, TaskList2RowKey), User2RowKey),
                                    new TaskListShareEntity(string.Format("{0}+{1}", TaskList2PartitionKey, TaskList2RowKey), User3RowKey)
                                };
        }

        #endregion Constructors

        #region Public methods

        public IQueryable<User> Load()
        {
            return Users.Select(n => n.MapToUser()).AsQueryable();
        }

        public User Get(string partitionKey, string rowKey)
        {
            var result = Users.FirstOrDefault(n => n.PartitionKey == partitionKey && n.RowKey == rowKey);
            return result != null ? result.MapToUser() : null;
        }

        public User Get(Expression<Func<User, bool>> filter)
        {
            var result = Users.Select(n => n.MapToUser()).AsQueryable().Where(filter).FirstOrDefault();
            return result;
        }

        public void Create(User entityToCreate)
        {
            var user = entityToCreate.MapToUserEntity();
            Users.Add(user);
        }

        public void Update(User entityToUpdate)
        {
            var user = entityToUpdate.MapToUserEntity();
            var userToRemove = Users.First(n => n.PartitionKey == user.PartitionKey && n.RowKey == user.RowKey);

            Users.Remove(userToRemove);
            Users.Add(user);
        }

        public void Delete(User entityToDelete)
        {
            Users.RemoveAll(n => n.PartitionKey == entityToDelete.PartitionKey && n.RowKey == entityToDelete.RowKey);
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

            return Users.FirstOrDefault(u => u.PartitionKey == identityProvider && u.UniqueIdentifier == uniqueIdentifier) != null;
        }

        public User GetByIdentifiers(string uniqueIdentifier, string identityProviderIdentifier)
        {
            var result = Users.FirstOrDefault(u => u.PartitionKey == identityProviderIdentifier && u.UniqueIdentifier == uniqueIdentifier);
            return result != null ? result.MapToUser() : null;
        }

        public void LoadOwner(Note note)
        {
            var owner = Users.FirstOrDefault(u => u.RowKey == note.PartitionKey);
            note.Owner = owner.MapToUser();
        }

        public void LoadShare(Note note)
        {
            var noteShares = NoteShares.Where(n => n.PartitionKey == string.Format("{0}+{1}", note.PartitionKey, note.RowKey));

            foreach (var noteShare in noteShares)
            {
                var user = Users.FirstOrDefault(u => u.RowKey == noteShare.RowKey);

                if (user != null)
                {
                    note.Share.Add(user.MapToUser());
                }
            }
        }

        public void LoadOwner(TaskList taskList)
        {
            var owner = Users.FirstOrDefault(u => u.RowKey == taskList.PartitionKey);
            taskList.Owner = owner.MapToUser();
        }

        public void LoadShare(TaskList taskList)
        {
            var taskListShares = TaskListShares.Where(n => n.PartitionKey == string.Format("{0}+{1}", taskList.PartitionKey, taskList.RowKey));

            foreach (var taskLisShare in taskListShares)
            {
                var user = Users.FirstOrDefault(u => u.RowKey == taskLisShare.RowKey);

                if (user != null)
                {
                    taskList.Share.Add(user.MapToUser());
                }
            }
        }

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

        #endregion Public methods
    }
}