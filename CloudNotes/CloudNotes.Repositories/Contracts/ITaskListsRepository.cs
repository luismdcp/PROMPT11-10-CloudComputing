using CloudNotes.Domain.Entities;

namespace CloudNotes.Repositories.Contracts
{
    public interface ITaskListsRepository : IRepository<TaskList>
    {
        void AddAssociatedUser(TaskList taskList, User userToAssociate);
        void DeleteAssociatedUser(TaskList taskList, User userToAssociate);
    }
}