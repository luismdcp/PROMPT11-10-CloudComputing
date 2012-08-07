using System.Linq;
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
            return _unitOfWork.Load<UserTableEntry>("Users").ToList().Select(userTableEntry => userTableEntry.MapToUser()).AsQueryable();
        }

        public User Get(string partitionKey, string rowKey)
        {
            var result = _unitOfWork.Get<UserTableEntry>("Users", partitionKey, rowKey);

            if (result != null)
            {
                return result.MapToUser();
            }

            return null;
        }

        public void Create(User entityToCreate)
        {
            entityToCreate.PartitionKey = "Users";
            entityToCreate.RowKey = entityToCreate.UserUniqueIdentifier;

            var userTableEntry = entityToCreate.MapToUserTableEntry();
            _unitOfWork.Create(userTableEntry, "Users");
        }

        public void Update(User entityToUpdate)
        {
            _unitOfWork.Update("Users", entityToUpdate.MapToUserTableEntry());
        }

        public void Delete(User entityToDelete)
        {
            _unitOfWork.Delete<UserTableEntry>("Users", entityToDelete.PartitionKey, entityToDelete.RowKey);
        }

        public bool UserExists(IPrincipal principal)
        {
            string userUniqueIdentifier = string.Empty;
            var claimsPrincipal = principal as IClaimsPrincipal;

            if (claimsPrincipal != null)
            {
                var claimsIdentity = claimsPrincipal.Identities[0];
                var nameIdentifierClaim = claimsIdentity.Claims.FirstOrDefault(c => c.ClaimType == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
                var identityProviderClaim = claimsIdentity.Claims.FirstOrDefault(c => c.ClaimType == "http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider");

                userUniqueIdentifier = string.Format("{0}_{1}", nameIdentifierClaim.Value, identityProviderClaim.Value);
            }

            var existingUserEntry = _unitOfWork.Get<UserTableEntry>("Users", "Users", userUniqueIdentifier);
            return existingUserEntry != null;
        }

        public User GetByUniqueIdentifier(string uniqueIdentifier)
        {
            var userEntry = _unitOfWork.Get<UserTableEntry>("Users", "Users", uniqueIdentifier);
            return userEntry.MapToUser();
        }

        public void LoadNoteOwner(Note note)
        {
            var noteOwnerEntry = _unitOfWork.Load<NoteOwnerTableEntry>("NoteOwner").Where(o => o.RowKey == note.RowKey).FirstOrDefault();

            if (noteOwnerEntry != null)
            {
                var owner = Get("Users", noteOwnerEntry.PartitionKey);
                note.Owner = owner;
            }
        }

        public void LoadNoteAssociatedUsers(Note note)
        {
            var noteAssociatedUsersEntries = _unitOfWork.Load<NoteAssociatedUserTableEntry>("NoteAssociatedUsers").Where(n => n.PartitionKey == note.RowKey);

            foreach (var noteAssociatedUserTableEntry in noteAssociatedUsersEntries)
            {
                var associatedUser = Get("Users", noteAssociatedUserTableEntry.RowKey);

                if (associatedUser != null)
                {
                    note.AssociatedUsers.Add(associatedUser);
                }
            }
        }

        public void LoadTaskListOwner(TaskList taskList)
        {
            var owner = Get("Users", taskList.PartitionKey);
            taskList.Owner = owner;
        }

        public void LoadTaskListAssociatedUsers(TaskList taskList)
        {
            var taskListAssociatedUsersEntries = _unitOfWork.Load<TaskListAssociatedUserTableEntry>("TaskListAssociatedUsers").Where(n => n.PartitionKey == taskList.RowKey);

            foreach (var taskListAssociatedUserTableEntry in taskListAssociatedUsersEntries)
            {
                var associatedUser = Get("Users", taskListAssociatedUserTableEntry.RowKey);

                if (associatedUser != null)
                {
                    taskList.AssociatedUsers.Add(associatedUser);
                }
            }
        }

        #endregion Public methods
    }
}