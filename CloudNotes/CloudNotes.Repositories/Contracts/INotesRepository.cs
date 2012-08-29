using CloudNotes.Domain.Entities;

namespace CloudNotes.Repositories.Contracts
{
    public interface INotesRepository : IRepository<Note>
    {
        bool NoteWithTitleExists(string title, TaskList container);
        void AddShare(Note note, string userId);
        void RemoveShare(Note note, string userId);
        void LoadNotes(TaskList taskList);
        bool HasPermissionToEdit(User user, Note note);
    }
}