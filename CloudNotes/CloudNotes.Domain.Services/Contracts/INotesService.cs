using CloudNotes.Domain.Entities;

namespace CloudNotes.Domain.Services.Contracts
{
    public interface INotesService : IService<Note>
    {
        bool NoteWithTitleExists(string title, TaskList container);
        void AddShare(Note note, string userId);
        void RemoveShare(Note note, string userId);
        void LoadNotes(TaskList taskList);
        void CopyNote(Note note, TaskList taskListDestination);
        void MoveNote(Note note, TaskList taskListDestination);
        bool HasPermissionToEdit(User user, Note note);
    }
}