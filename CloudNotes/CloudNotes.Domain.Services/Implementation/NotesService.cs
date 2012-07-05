using System.Linq;
using CloudNotes.Domain.Entities;
using CloudNotes.Domain.Services.Contracts;
using CloudNotes.Repositories.Contracts;
using CloudNotes.Repositories.Implementation;

namespace CloudNotes.Domain.Services.Implementation
{
    public class NotesService : INotesService
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly INotesRepository _repository;

        #endregion Fields

        #region Constructors

        public NotesService()
        {
            _unitOfWork = new AzureTablesUnitOfWork();
            _repository = new NotesRepository(_unitOfWork);
        }

        #endregion Constructors

        #region Public methods

        public IQueryable<Note> Load()
        {
            return _repository.Load();
        }

        public Note Get(string partitionKey, string rowKey)
        {
            return _repository.Get(partitionKey, rowKey);
        }

        public void Add(Note entityToAdd)
        {
            _repository.Add(entityToAdd);
            _unitOfWork.SubmitChanges();
        }

        public void Update(Note entityToUpdate)
        {
            _repository.Update(entityToUpdate);
            _unitOfWork.SubmitChanges();
        }

        public void Delete(Note entityToDelete)
        {
            _repository.Delete(entityToDelete);
            _unitOfWork.SubmitChanges();
        }

        public void AddAssociatedUser(Note note, User associatedUser)
        {
            note.AssociatedUsers.Add(associatedUser);
            _repository.AddAssociatedUser(note, associatedUser);
            _unitOfWork.SubmitChanges();
        }

        public void DeleteAssociatedUser(Note note, User associatedUser)
        {
            note.AssociatedUsers.Remove(associatedUser);
            _repository.DeleteAssociatedUser(note, associatedUser);
            _unitOfWork.SubmitChanges();
        }

        #endregion Public methods
    }
}