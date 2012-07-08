using System.Linq;
using CloudNotes.Domain.Entities;
using CloudNotes.Domain.Services.Contracts;
using CloudNotes.Repositories.Contracts;

namespace CloudNotes.Domain.Services.Implementation
{
    public class UsersService : IUsersService
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly IUsersRepository _repository;

        #endregion Fields

        #region Constructors

        public UsersService(IUnitOfWork unitOfWork, IUsersRepository usersRepository)
        {
            _unitOfWork = unitOfWork;
            _repository = usersRepository;
        }

        #endregion Constructors

        #region Public methods

        public IQueryable<User> Load()
        {
            return _repository.Load();
        }

        public User Get(string partitionKey, string rowKey)
        {
            return _repository.Get(partitionKey, rowKey);
        }

        public void Add(User entityToAdd)
        {
            _repository.Create(entityToAdd);
            _unitOfWork.SubmitChanges();
        }

        public void Update(User entityToUpdate)
        {
            _repository.Update(entityToUpdate);
            _unitOfWork.SubmitChanges();
        }

        public void Delete(User entityToDelete)
        {
            _repository.Delete(entityToDelete);
            _unitOfWork.SubmitChanges();
        }

        public User ManageUser()
        {
            var user = _repository.GetOrAddCurrentUser();
            _unitOfWork.SubmitChanges();
            return user;
        }

        #endregion Public methods
    }
}