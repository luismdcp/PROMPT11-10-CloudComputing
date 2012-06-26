using CloudNotes.Domain.Entities;

namespace CloudNotes.Domain.Services.Contracts
{
    public interface ITaskListsService : IService<TaskList>
    {
        void AddAssociatedUser(TaskList taskList, User associatedUser);
        void DeleteAssociatedUser(TaskList taskList, User associatedUser);
        void CopyNote(TaskList taskListSource, TaskList taskListDestination, Note note);
        void MoveNote(TaskList taskListSource, TaskList taskListDestination, Note note);
    }
}