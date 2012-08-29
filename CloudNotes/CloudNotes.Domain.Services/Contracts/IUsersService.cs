using System.Security.Principal;
using CloudNotes.Domain.Entities;
using CloudNotes.Domain.Services.Models;

namespace CloudNotes.Domain.Services.Contracts
{
    public interface IUsersService : IService<User>
    {
        User Get(string rowKey);
        bool UserIsRegistered(IPrincipal principal);
        User GetByIdentifiers(string uniqueIdentifier, string identityProviderIdentifier);
        AuthenticationInfo GetUserAuthenticationInfo(IPrincipal principal);
        void FillAuthenticationInfo(User user, IPrincipal principal);
        void LoadOwner(Note note);
        void LoadOwner(TaskList taskList);
        void LoadShare(Note note);
        void LoadShare(TaskList taskList);
    }
}