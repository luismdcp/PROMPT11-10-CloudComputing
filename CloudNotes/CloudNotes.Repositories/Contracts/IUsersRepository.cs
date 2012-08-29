using System.Security.Principal;
using CloudNotes.Domain.Entities;

namespace CloudNotes.Repositories.Contracts
{
    public interface IUsersRepository : IRepository<User>
    {
        bool UserIsRegistered(IPrincipal principal);
        User GetByIdentifiers(string uniqueIdentifier, string identityProviderIdentifier);
        void LoadOwner(Note note);
        void LoadShare(Note note);
        void LoadOwner(TaskList taskList);
        void LoadShare(TaskList taskList);
    }
}