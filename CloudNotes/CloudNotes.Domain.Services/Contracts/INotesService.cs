using CloudNotes.Domain.Entities;

namespace CloudNotes.Domain.Services.Contracts
{
    public interface INotesService : IService<Note>
    {
        void AddAssociatedUser(Note note, User associatedUser);
        void DeleteAssociatedUser(Note note, User associatedUser);
    }
}