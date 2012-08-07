using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using CloudNotes.Domain.Entities;
using CloudNotes.Repositories.Contracts;
using CloudNotes.Repositories.Entities;
using CloudNotes.Repositories.Entities.Relation;
using CloudNotes.Repositories.Extensions;
using Microsoft.IdentityModel.Claims;

namespace CloudNotes.Tests.Domain.Services.Memory
{
    public class UsersMemoryRepository : IUsersRepository
    {
        #region Fields

        private readonly List<UserTableEntry> _usersTableEntries;
        private readonly List<NoteOwnerTableEntry> _noteOwnerTableEntries;
        private readonly List<NoteAssociatedUserTableEntry> _noteAssociatedUsersTableEntries;
        private readonly List<TaskListOwnerTableEntry> _taskListOwnerTableEntries;
        private readonly List<TaskListAssociatedUserTableEntry> _taskListAssociatedUsersTableEntries;

        #endregion Fields

        #region Constructors

        public UsersMemoryRepository()
        {
            _usersTableEntries = new List<UserTableEntry>
                                 {
                                     new UserTableEntry("users", "user1"), 
                                     new UserTableEntry("users", "user2_testIdentityProvider"),
                                     new UserTableEntry("users", "user3")
                                 };

            _noteOwnerTableEntries = new List<NoteOwnerTableEntry>
                                         {
                                             new NoteOwnerTableEntry("user1", "note1"),
                                             new NoteOwnerTableEntry("user2", "note2")
                                         };

            _taskListOwnerTableEntries = new List<TaskListOwnerTableEntry>
                                         {
                                             new TaskListOwnerTableEntry("user1", "taskList1"),
                                             new TaskListOwnerTableEntry("user2", "taskList2")
                                         };

            _noteAssociatedUsersTableEntries = new List<NoteAssociatedUserTableEntry>
                                                   {
                                                       new NoteAssociatedUserTableEntry("note1", "user1"),
                                                       new NoteAssociatedUserTableEntry("note1", "user3"),
                                                       new NoteAssociatedUserTableEntry("note2", "user2")
                                                   };

            _taskListAssociatedUsersTableEntries = new List<TaskListAssociatedUserTableEntry>
                                                   {
                                                       new TaskListAssociatedUserTableEntry("taskList1", "user1"),
                                                       new TaskListAssociatedUserTableEntry("taskList1", "user3"),
                                                       new TaskListAssociatedUserTableEntry("taskList2", "user2")
                                                   };
        }

        #endregion Constructors

        #region Public methods

        public IQueryable<User> Load()
        {
            return _usersTableEntries.Select(n => n.MapToUser()).AsQueryable();
        }

        public User Get(string partitionKey, string rowKey)
        {
            var result = _usersTableEntries.FirstOrDefault(n => n.PartitionKey == partitionKey && n.RowKey == rowKey);
            return result != null ? result.MapToUser() : null;
        }

        public void Create(User entityToCreate)
        {
            var userTableEntry = entityToCreate.MapToUserTableEntry();
            _usersTableEntries.Add(userTableEntry);
        }

        public void Update(User entityToUpdate)
        {
            var userTableEntry = entityToUpdate.MapToUserTableEntry();
            var userTableEntryToRemove = _usersTableEntries.First(n => n.PartitionKey == userTableEntry.PartitionKey && n.RowKey == userTableEntry.RowKey);

            _usersTableEntries.Remove(userTableEntryToRemove);
            _usersTableEntries.Add(userTableEntry);
        }

        public void Delete(User entityToDelete)
        {
            _usersTableEntries.RemoveAll(n => n.PartitionKey == entityToDelete.PartitionKey && n.RowKey == entityToDelete.RowKey);
        }

        public User GetOrAddCurrentUser(IPrincipal principal)
        {
            string userUniqueIdentifier = string.Empty;
            string userEmailAddress = string.Empty;
            var claimsPrincipal = principal as IClaimsPrincipal;

            if (claimsPrincipal != null)
            {
                var claimsIdentity = claimsPrincipal.Identities[0];
                var nameIdentifierClaim = claimsIdentity.Claims.FirstOrDefault(claim => claim.ClaimType == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
                var emailAddressClaim = claimsIdentity.Claims.FirstOrDefault(claim => claim.ClaimType == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");
                var identityProviderClaim = claimsIdentity.Claims.FirstOrDefault(claim => claim.ClaimType == "http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider");

                userUniqueIdentifier = string.Format("{0}_{1}", nameIdentifierClaim.Value, identityProviderClaim.Value);
                userEmailAddress = emailAddressClaim == null ? string.Empty : emailAddressClaim.Value;
            }
            else
            {
                userUniqueIdentifier = principal.Identity.Name;
            }

            var newOrExistingUserEntry = _usersTableEntries.FirstOrDefault(u => u.RowKey == userUniqueIdentifier);

            if (newOrExistingUserEntry == null)
            {
                var user = new User("users", userUniqueIdentifier, userUniqueIdentifier, userEmailAddress);
                Create(user);
                return user;
            }

            return newOrExistingUserEntry.MapToUser();
        }

        public void LoadNoteOwner(Note note)
        {
            var noteOwnerEntry = _noteOwnerTableEntries.FirstOrDefault(n => n.RowKey == note.RowKey);

            if (noteOwnerEntry != null)
            {
                var noteOwner = _usersTableEntries.FirstOrDefault(u => u.RowKey == noteOwnerEntry.PartitionKey);

                if (noteOwner != null)
                {
                    note.Owner = noteOwner.MapToUser();
                }
            }
        }

        public void LoadNoteAssociatedUsers(Note note)
        {
            var noteAssociatedUsers = _noteAssociatedUsersTableEntries.Where(n => n.PartitionKey == note.RowKey);

            foreach (var noteAssociatedUserTableEntry in noteAssociatedUsers)
            {
                var noteAssociatedUser = _usersTableEntries.FirstOrDefault(u => u.RowKey == noteAssociatedUserTableEntry.RowKey);

                if (noteAssociatedUser != null)
                {
                    note.AssociatedUsers.Add(noteAssociatedUser.MapToUser());
                }
            }
        }

        public void LoadTaskListOwner(TaskList taskList)
        {
            var taskListOwnerEntry = _taskListOwnerTableEntries.FirstOrDefault(n => n.RowKey == taskList.RowKey);

            if (taskListOwnerEntry != null)
            {
                var taskListOwner = _usersTableEntries.FirstOrDefault(u => u.RowKey == taskListOwnerEntry.PartitionKey);

                if (taskListOwner != null)
                {
                    taskList.Owner = taskListOwner.MapToUser();
                }
            }
        }

        public void LoadTaskListAssociatedUsers(TaskList taskList)
        {
            var taskListAssociatedUsers = _taskListAssociatedUsersTableEntries.Where(n => n.PartitionKey == taskList.RowKey);

            foreach (var taskListAssociatedUserTableEntry in taskListAssociatedUsers)
            {
                var taskListAssociatedUser = _usersTableEntries.FirstOrDefault(u => u.RowKey == taskListAssociatedUserTableEntry.RowKey);

                if (taskListAssociatedUser != null)
                {
                    taskList.AssociatedUsers.Add(taskListAssociatedUser.MapToUser());
                }
            }
        }

        #endregion Public methods
    }
}