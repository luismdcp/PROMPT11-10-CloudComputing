using CloudNotes.Domain.Entities;

namespace CloudNotes.Repositories.Contracts
{
    public interface INotesRepository : IRepository<Note>
    {
        void AddAssociatedUser(Note note, User userToAssociate);
        void DeleteAssociatedUser(Note note, User userToAssociate);
    }
}