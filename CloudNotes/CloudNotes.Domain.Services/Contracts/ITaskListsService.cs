using System.Collections.Generic;
using CloudNotes.Domain.Entities;

namespace CloudNotes.Domain.Services.Contracts
{
    public interface ITaskListsService : IService<TaskList>
    {
        IEnumerable<TaskList> GetShared(User user);
        TaskList Get(string combinedKeys);
        void LoadContainer(Note note);
        void AddShare(TaskList taskList, string userId);
        void RemoveShare(TaskList taskList, string userId);
        bool HasPermissionToEdit(User user, TaskList taskList);
    }
}