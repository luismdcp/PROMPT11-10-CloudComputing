using System.Linq;
using CloudNotes.Domain.Entities;
using CloudNotes.Repositories.Contracts;
using CloudNotes.Repositories.Entities;
using CloudNotes.Repositories.Extensions;

namespace CloudNotes.Repositories.Implementation
{
    public class UsersRepository : IUsersRepository
    {
        #region Fields

        private readonly TableDataContext _unitOfWork;
        private readonly TableRepository<UserTableEntry> _userTableRepository;

        #endregion Fields

         #region Constructors

        public UsersRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork as TableDataContext;
            _userTableRepository = new TableRepository<UserTableEntry>("Users", _unitOfWork);
        }

        #endregion Constructors

        #region Public methods

        public IQueryable<User> Load()
        {
            var users = _userTableRepository.Load().ToList();
            return users.Select(userTableEntry => userTableEntry.MapToUser()).AsQueryable();
        }

        public User Get(string partitionKey, string rowKey)
        {
            return _userTableRepository.Get(partitionKey, rowKey).MapToUser();
        }

        public void Add(User entityToAdd)
        {
            var userTableEntry = entityToAdd.MapToUserTableEntry();
            _userTableRepository.Add(userTableEntry);
        }

        public void Update(User entityToUpdate)
        {
            var userTableEntry = entityToUpdate.MapToUserTableEntry();
            _userTableRepository.Update(userTableEntry);
        }

        public void Delete(User entityToDelete)
        {
            var userTableEntry = entityToDelete.MapToUserTableEntry();
            _userTableRepository.Delete(userTableEntry);
        }

        #endregion Public methods
    }
}