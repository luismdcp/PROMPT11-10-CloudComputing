using CloudNotes.Domain.Entities;

namespace CloudNotes.Repositories.Contracts
{
    public interface INotesRepository : IRepository<Note>
    {
        Note GetByTitle(string title, TaskList containerTaskList);
        Note GetByTitle(string title, string containerTaskListRowKey);
        void AddAssociatedUser(Note note, User userToAdd);
        void RemoveAssociatedUser(Note note, User userToRemove);
        void LoadNotes(TaskList taskList);
    }
}