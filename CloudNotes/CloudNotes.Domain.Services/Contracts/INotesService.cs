using CloudNotes.Domain.Entities;

namespace CloudNotes.Domain.Services.Contracts
{
    public interface INotesService : IService<Note>
    {
        Note GetByTitle(string title, TaskList containerList);
        Note GetByTitle(string title, string containerTaskListRowKey);
        Note GetNoteEagerLoaded(string taskListTitle, string noteTitle, User user);
        void AddAssociatedUser(Note note, User associatedUser);
        void RemoveAssociatedUser(Note note, User associatedUser);
        void LoadNotes(TaskList taskList);
        void CopyNote(Note note, TaskList taskListDestination);
        void MoveNote(Note note, TaskList taskListDestination);
    }
}