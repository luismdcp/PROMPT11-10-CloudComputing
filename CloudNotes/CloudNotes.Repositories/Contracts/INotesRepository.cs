using CloudNotes.Domain.Entities;

namespace CloudNotes.Repositories.Contracts
{
    public interface INotesRepository : IRepository<Note>
    {
        void AddAssociatedUser(Note note, User associatedUser);
        void DeleteAssociatedUser(Note note, User associatedUser);
        void AddOrReplaceAssociatedUsers(Note note);
    }
}