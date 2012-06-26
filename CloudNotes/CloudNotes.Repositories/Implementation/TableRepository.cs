using System.Linq;
using CloudNotes.Repositories.Contracts;
using CloudNotes.Repositories.Entities;

namespace CloudNotes.Repositories.Implementation
{
    public class TableRepository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
    {
        #region Fields

        private readonly string _entitySetName;
        private readonly TableDataContext _dataContext;

        #endregion Fields

        #region Constructors

        public TableRepository(string entitySetName, IUnitOfWork unitOfWork)
        {
            _entitySetName = entitySetName;
            _dataContext = unitOfWork as TableDataContext;
        }

        #endregion Constructors

        #region Public methods

        public IQueryable<TEntity> Load()
        {
            return _dataContext.CreateQuery<TEntity>(_entitySetName);
        }

        public TEntity Get(string partitionKey, string rowKey)
        {
            return _dataContext.CreateQuery<TEntity>(_entitySetName).FirstOrDefault(n => n.PartitionKey == partitionKey && n.RowKey == rowKey);
        }

        public void Add(TEntity entityToAdd)
        {
            _dataContext.AddObject(_entitySetName, entityToAdd);
        }

        public void Update(TEntity entityToUpdate)
        {
            _dataContext.UpdateObject(entityToUpdate);
        }

        public void Delete(TEntity entityToDelete)
        {
            _dataContext.DeleteObject(entityToDelete);
        }

        #endregion Public methods
    }
}