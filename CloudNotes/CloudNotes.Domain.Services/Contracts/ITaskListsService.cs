using System.Collections.Generic;
using CloudNotes.Domain.Entities;

namespace CloudNotes.Domain.Services.Contracts
{
    public interface ITaskListsService : IService<TaskList>
    {
        TaskList GetByTitleAndOwner(string title, User owner);
        TaskList GetTaskListEagerLoaded(string taskListTitle, User user);
        IEnumerable<TaskList> GetTaskListsUserIsAssociated(User user);
        IEnumerable<TaskList> GetTaskListsOwnedByUser(User user);
        void AddAssociatedUser(TaskList taskList, User associatedUser);
        void RemoveAssociatedUser(TaskList taskList, User associatedUser);
    }
}