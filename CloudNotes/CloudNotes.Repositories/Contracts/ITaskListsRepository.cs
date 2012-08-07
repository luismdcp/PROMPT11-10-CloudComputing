using System.Collections.Generic;
using CloudNotes.Domain.Entities;

namespace CloudNotes.Repositories.Contracts
{
    public interface ITaskListsRepository : IRepository<TaskList>
    {
        TaskList GetByTitleAndOwner(string title, User owner);
        IEnumerable<TaskList> GetTaskListsAssociatedByUser(User user);
        IEnumerable<TaskList> GetTaskListsOwnedByUser(User user);
        void AddAssociatedUser(TaskList taskList, User userToAdd);
        void RemoveAssociatedUser(TaskList taskList, User userToRemove);
    }
}