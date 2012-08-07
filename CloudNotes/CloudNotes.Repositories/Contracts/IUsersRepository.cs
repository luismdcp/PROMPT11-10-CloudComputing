using System.Security.Principal;
using CloudNotes.Domain.Entities;

namespace CloudNotes.Repositories.Contracts
{
    public interface IUsersRepository : IRepository<User>
    {
        bool UserExists(IPrincipal principal);
        User GetByUniqueIdentifier(string uniqueIdentifier);
        void LoadNoteOwner(Note note);
        void LoadNoteAssociatedUsers(Note note);
        void LoadTaskListOwner(TaskList taskList);
        void LoadTaskListAssociatedUsers(TaskList taskList);
    }
}