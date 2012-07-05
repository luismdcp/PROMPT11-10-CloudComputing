using System.Linq;
using CloudNotes.Repositories.Contracts;
using CloudNotes.Repositories.Entities;

namespace CloudNotes.Repositories.Implementation
{
    public class TableRepository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
    {
        #region Fields

        private readonly string _entitySetName;
        private readonly IUnitOfWork _unitOfWork;

        #endregion Fields

        #region Constructors

        public TableRepository(string entitySetName, IUnitOfWork unitOfWork)
        {
            _entitySetName = entitySetName;
            _unitOfWork = unitOfWork;
        }

        #endregion Constructors

        #region Public methods

        public IQueryable<TEntity> Load()
        {
            return _unitOfWork.Load<TEntity>(_entitySetName);
        }

        public TEntity Get(string partitionKey, string rowKey)
        {
            return _unitOfWork.Get<TEntity>(_entitySetName, partitionKey, rowKey);
        }

        public void Add(TEntity entityToAdd)
        {
            _unitOfWork.Add(entityToAdd, _entitySetName);
        }

        public void Update(TEntity entityToUpdate)
        {
            _unitOfWork.Update(entityToUpdate);
        }

        public void Delete(TEntity entityToDelete)
        {
            _unitOfWork.Delete(entityToDelete);
        }

        #endregion Public methods
    }
}