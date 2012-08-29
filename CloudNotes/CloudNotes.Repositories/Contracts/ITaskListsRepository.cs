using System.Collections.Generic;
using CloudNotes.Domain.Entities;

namespace CloudNotes.Repositories.Contracts
{
    public interface ITaskListsRepository : IRepository<TaskList>
    {
        IEnumerable<TaskList> GetShared(User user);
        void AddShare(TaskList taskList, string userId);
        void RemoveShare(TaskList taskList, string userId);
        void LoadContainer(Note note);
        bool HasPermissionToEdit(User user, TaskList taskList);
    }
}