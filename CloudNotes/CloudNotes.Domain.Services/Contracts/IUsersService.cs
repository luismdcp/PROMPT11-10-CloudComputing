using System.Security.Principal;
using CloudNotes.Domain.Entities;

namespace CloudNotes.Domain.Services.Contracts
{
    public interface IUsersService : IService<User>
    {
        bool UserExists(IPrincipal principal);
        User GetByUniqueIdentifier(string uniqueIdentifier);
        void GetUserAuthenticationInfo(IPrincipal principal, out string name, out string email, out string uniqueIdentifier);
        void FillUniqueIdentifier(User user, IPrincipal principal);
        void LoadNoteOwner(Note note);
        void LoadTaskListOwner(TaskList taskList);
        void LoadNoteAssociatedUsers(Note note);
        void LoadTaskListAssociatedUsers(TaskList taskList);
    }
}